﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyService.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MyServiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PaymentController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = sign;
            _userManager = userManager;
        }

        // GET: Payment/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            // Use a proper property (e.g., UserName) to filter the payments.
            var payment = await _context.paids
                .Include(p => p.Request)
                .ThenInclude(r => r.Service)
                .FirstOrDefaultAsync(p => p.Request.UserId == identityUser.Id);

            
            // Auto-fill the price based on the service's predetermined pricing.

            return View(payment);
        }

        // POST: Payment/ProcessPayment
        // Updates the payment record (for instance, when the user enters the amount manually)


        // POST: Payment/ConfirmPayment
        // Marks the payment as confirmed by setting the makepaid flag to true.

        // GET: Payment/pymentlist
        // Returns a list of payment records for the current user.
        public async Task<IActionResult> pymentlist()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Customer");
            }
            var identityUser = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(identityUser, Helper.Roles.SuperAdmin.ToString()))
            {
                if (CultureInfo.CurrentCulture.Name.StartsWith("ar"))
                {
                    return RedirectToAction("Indexar", "Home");

                } 
            

            return RedirectToAction("Index", "Home");
            }
            var payments = await _context.paids
                .Include(p => p.Request)
                .ThenInclude(r => r.Service)
                .Where(p => p.Request.UserId == identityUser.UserName)
                .ToListAsync();


            

            // Update the amount for each payment based on the predetermined price.


            return View(payments);
        }
        public async Task<IActionResult> DeleteAll()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var paidslist = await _context.paids
                .Where(n => n.Request.UserId == currentUser.ToString())
                .ToListAsync();

            if (!paidslist.Any())
            {
                return Json(new { success = false, message = "No Orders found." });
            }

            _context.paids.RemoveRange(paidslist);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All Orders deleted successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.paids.FindAsync(id);

            if (payment == null)
            {
                return Json(new { success = false, message = "Payment record not found." });
            }

            // Create a new history record from the payment data
            var historyRecord = new historypaid()
            {
                // If historypaid has its own identity generation, you may omit setting Id.
                // Otherwise, if you want to preserve the same Id, ensure it doesn't conflict.
                // Id = payment.Id, 
                amount = payment.amount,
                DateTime = payment.DateTime,
                RequestId = payment.RequestId,
                makepaid = payment.makepaid,
                NameServece = payment.NameServece
            };

            await _context.historypaids.AddAsync(historyRecord);

            // Remove the original payment record
            _context.paids.Remove(payment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Payment deleted successfully and saved in history." });
        }

    }
}
