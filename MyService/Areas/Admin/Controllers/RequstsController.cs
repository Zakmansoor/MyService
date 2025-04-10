﻿using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RequstsController : Controller
    {
        private readonly MyServiceDbContext _context;
        public RequstsController(MyServiceDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Index()
        {
            var requests = await _context.requests
                .Include(r => r.Service)
                .ToListAsync();

            return View(requests);
        } 

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _context.requests.FindAsync(id);
            if (request == null)
                return NotFound();

            var viewModel = new RequestViewModel
            {
                RequestId = request.RequestId,
                ServiceId = request.ServiceId,
                OrderDate = request.OrderDate,
                Status = request.Status,
                Comment = request.Comment,
                Customers = _context.customers.ToList(),
                Services = _context.services.ToList()
            };

            return View(viewModel);
        }

        // POST: Request/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Edit(int id, RequestViewModel viewModel)
        {
            if (id != viewModel.RequestId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var request = await _context.requests.FindAsync(id);
                    request.ServiceId = viewModel.ServiceId;
                    request.OrderDate = viewModel.OrderDate;
                    request.Status = viewModel.Status;
                    request.Comment = viewModel.Comment;

                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(viewModel.RequestId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.Services = _context.services.ToList();
            return View(viewModel);
        }

        // (Optional) GET: Request/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var request = await _context.requests
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);

            if (request == null)
                return NotFound();

            return View(request);
        }

        // AJAX-based single delete action
        [HttpPost]
        public async Task<JsonResult> DeleteRequest(int id)
        {
            var request = await _context.requests.FindAsync(id);
            if (request == null)
                return Json(new { success = false, message = "Request not found" });

            _context.requests.Remove(request);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // Original DeleteConfirmed action, kept for non-AJAX usage if needed.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.requests.FindAsync(id);
            _context.requests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.requests.Any(e => e.RequestId == id);
        }

       
        [HttpPost]
        public async Task<JsonResult> MarkAsPaid(int id)
        {
            var bill = await _context.requests.FindAsync(id);
            if (bill == null)
                return Json(new { success = false, message = "Request not found" });

            // Mark the bill as paid.
            bill.Status = true;

            // Create a notification.
            // Note: Replace the placeholder empId with the actual employee ID
            // (e.g., by retrieving it from the logged-in user if using Identity).
            Notification notification = new Notification
            {
                // Assign the user id from the request or, if needed, from the current logged-in user.
                UserId = bill.UserId,
                // Customize the message as required. Here we're using the request id.
                Message = $"Request {bill.RequestId} has been marked as paid.",
                IsRead = false,
                CreatedAt = DateTime.Now,
                // Set the foreign key linking back to the Request.
                requestId = bill.RequestId,
                // Set a placeholder employee id. Replace this with the actual employee id.
             
            };

            // Add the notification to the context.
            _context.notifications.Add(notification);

            // Save both the updated request and the new notification.
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var allRequests = await _context.requests.ToListAsync();
                _context.requests.RemoveRange(allRequests);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return Json(new { success = false, error = ex.Message });
            }
        }

    }
}
