using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WorkersController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MyServiceDbContext _context;

        public WorkersController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MyServiceDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;

            _context = context;
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Index()
        {
            var model = new EmployeesViewModel
            {
                NewEmployees = new NewEmployees(),
                Employees = _context.employees.Include(e=>e.Service).ToList() ?? new List<Employees>(), // Handle null
                services = _context.services?.ToList() ?? new List<Service>()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEmployee(EmployeesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.services = _context.services?.ToList() ?? new List<Service>();
                return View("Index", model);
            }

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

                if (model.NewEmployees.Id == 0) // Create new employee
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.NewEmployees.Email,
                        Email = model.NewEmployees.Email,
                        Name = model.NewEmployees.Name,
                        ActiveUser = model.NewEmployees.IsActive
                    };

                    // Create the user first
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        TempData["Error"] = "Failed to create user: " + string.Join(", ", createResult.Errors.Select(e => e.Description));
                        return RedirectToAction("Index");
                    }

                    // Assign role after user is created
                    var roleResult = await _userManager.AddToRoleAsync(user, Helper.Roles.Admin.ToString());
                    if (!roleResult.Succeeded)
                    {
                        TempData["Error"] = "Failed to assign role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        return RedirectToAction("Index");
                    }

                    // Create employee linked to the user
                    var newEmployee = new Employees
                    {
                        Name = model.NewEmployees.Name,
                        Email = model.NewEmployees.Email,
                        PhoneNumber = model.NewEmployees.Phone,
                        Created = DateTime.Now,
                        IsActive = model.NewEmployees.IsActive,
                        TitleResume = model.NewEmployees.TitleResume,
                        UserId = user.UserName, // Use user.Id, not Email
                        SeID = model.NewEmployees.servisId
                    };

                    await _context.employees.AddAsync(newEmployee);
                }
                else // Update existing employee
                {
                    var employee = await _context.employees.FindAsync(model.NewEmployees.Id);
                    if (employee == null)
                    {
                        TempData["Error"] = "Employee not found.";
                        return RedirectToAction("Index");
                    }

                    employee.Name = model.NewEmployees.Name;
                    employee.Email = model.NewEmployees.Email;
                    employee.IsActive = model.NewEmployees.IsActive;
                    employee.SeID = model.NewEmployees.servisId; // Update service ID

                    // Update resume only if a new file is uploaded
                    if (!string.IsNullOrEmpty(model.NewEmployees.TitleResume))
                    {
                        employee.TitleResume = model.NewEmployees.TitleResume;
                    }

                    _context.employees.Update(employee);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Employee saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                model.services = _context.services?.ToList() ?? new List<Service>();
                return View("Index", model);
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
