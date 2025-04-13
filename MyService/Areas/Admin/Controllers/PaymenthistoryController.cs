using System;
using System.Linq;
using System.Threading.Tasks;
using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyService.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PaymenthistoryController : Controller
    {
        private readonly MyServiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PaymenthistoryController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = sign;
            _userManager = userManager;
        }

        // GET: Payment/Index
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Index()
        {

            var identityUser = await _userManager.GetUserAsync(User);

            var payments = await _context.historypaids
                .Include(p => p.Request)
                .ThenInclude(r => r.Service)

                .ToListAsync();

            if (payments == null || !payments.Any())
            {
                TempData["Error"] = "No payment record found.";
                return RedirectToAction("Index", "Home");
            }

            // Update the amount for each payment based on the predetermined price.


            return View(payments);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                var allhistorypaids = await _context.historypaids.ToListAsync();
                _context.historypaids.RemoveRange(allhistorypaids);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return Json(new { success = false, error = ex.Message });
            }
        }
        // POST: Payment/ProcessPayment
        // Updates the payment record (for instance, when the user enters the amount manually)

    }
}
