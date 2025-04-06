﻿using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WorkersController : Controller
    {
        private readonly MyServiceDbContext _context;

        public WorkersController(MyServiceDbContext context, ILogger<WorkersController> logger)
        {
            _context = context;
        }
        [Authorize(Roles = "SuperAdmin")]

        public IActionResult Index()
        {
            var model = new EmployeesViewModel
            {
                NewEmployees = new NewEmployees(),
                Employees = _context.employees.Include(a=>a.employeeRequests).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveEmployee(EmployeesViewModel model)
        {


            try
            {
                // File handling
                var file = HttpContext.Request.Form.Files.FirstOrDefault();
                if (file != null && file.Length > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Web/resumes");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    model.NewEmployees.TitleResume = fileName;
                }

                // Create or update logic
                if (model.NewEmployees.Id == 0) // Create new employee
                {
                    var newEmployee = new Employees
                    {
                        Name = model.NewEmployees.Name,
                        Email = model.NewEmployees.Email,
                        PhoneNumber = model.NewEmployees.Phone,
                        Created = DateTime.Now,
                        IsActive = model.NewEmployees.IsActive,
                        TitleResume = model.NewEmployees.TitleResume
                    };

                    await _context.employees.AddAsync(newEmployee);
                }
                else
                {
                    var employee = await _context.employees.FindAsync(model.NewEmployees.Id);
                    if (employee != null)
                    {
                        employee.Name = model.NewEmployees.Name;
                        employee.Email = model.NewEmployees.Email;
                        employee.PhoneNumber = model.NewEmployees.Phone;
                        employee.Created = model.NewEmployees.Date;
                        employee.IsActive = model.NewEmployees.IsActive;


                        // Update resume file only if a new one was uploaded
                        if (!string.IsNullOrEmpty(model.NewEmployees.TitleResume))
                        {
                            employee.TitleResume = model.NewEmployees.TitleResume;
                        }

                        _context.employees.Update(employee);
                    }
                    else
                    {
                        TempData["Error"] = "Employee not found.";
                        return RedirectToAction("Index");
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Employee saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.employees.FindAsync(id);
                if (employee != null)
                {
                    _context.employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Employee deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Employee not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                TempData["Error"] = "File name not provided.";
                return RedirectToAction("Index");
            }

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Web/resumes", fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    TempData["Error"] = "File not found.";
                    return RedirectToAction("Index");
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileType = "application/octet-stream";
                return File(fileStream, fileType, fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

