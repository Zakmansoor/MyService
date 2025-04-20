using Domin.Entity;
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
      
            #region Declaration
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly MyServiceDbContext _context;
            #endregion

            #region Constructor
            public RequstsController(RoleManager<IdentityRole> roleManager,
                UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MyServiceDbContext context)
            {
                _roleManager = roleManager;
                _userManager = userManager;
                _signInManager = signInManager;
                _context = context;
            }
        #endregion
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Indexem()
        {
            try
            {
                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized(); // Or redirect to login
                }

                // Get employee record
                var currentEmployee = await _context.employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.UserId == currentUser.UserName);

                if (currentEmployee == null)
                {
                    return NotFound("Employee record not found");
                }

                // Get requests for employee's service
                var requests = await _context.requests
                    .Include(r => r.Service)
                    .Include(r => r.Mapping)
                    .Where(r => r.ServiceId == currentEmployee.SeID)
                    .ToListAsync();

                return View(requests);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred");
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]

        public async Task<IActionResult> Index()
        {
            var requests = await _context.requests
                .Include(r => r.Service)
                .Include(r => r.Mapping)
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
                ServiceId = request.ServiceId,
                OrderDate = request.OrderDate,
                Status = request.Status,
                Comment = request.Comment,
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
            if (id != viewModel.ServiceId)
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
                    if (!RequestExists(viewModel.ServiceId))
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
            // 1. Load the request *and* its related historypaid entry (if any)
            var bill = await _context.requests
                         .Include(r => r.historypaid)
                         .Include(r => r.Service)
                         .FirstOrDefaultAsync(r => r.RequestId == id);
            if (bill == null)
                return Json(new { success = false, message = "Request not found" });

            // 2. Mark the request itself as paid
            bill.Status = true;

            // 3. If there's no historypaid yet, create one; otherwise update it
            if (bill.historypaid == null)
            {
                bill.historypaid = new historypaid
                {
                    RequestId = bill.RequestId,
                    amount = bill.Service?.Requests.Count() > 0
                                       ? bill.Service.Requests.Count() // or however you compute amount
                                       : 0,
                    DateTime = DateTime.Now,
                    makepaid = true,
                    NameServece = bill.Service?.Name // record the service’s name
                };
                _context.historypaids.Add(bill.historypaid);
            }
            else
            {
                // 4. Update the existing historypaid entry
                bill.historypaid.makepaid = true;
                bill.historypaid.DateTime = DateTime.Now;
                bill.historypaid.NameServece = bill.Service?.Name;
                _context.historypaids.Update(bill.historypaid);
            }

            // 5. Create a notification for the user (unchanged)
            var notification = new Notification
            {
                UserId = bill.UserId,
                Message = $"Request {bill.RequestId} has been marked as paid.",
                IsRead = false,
                CreatedAt = DateTime.Now,
                requestId = bill.RequestId,
            };
            _context.notifications.Add(notification);

            // 6. Persist *all* of the above in one round‑trip
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
