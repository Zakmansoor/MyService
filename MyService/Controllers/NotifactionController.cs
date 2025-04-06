using System.Linq;
using System.Threading.Tasks;
using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyService.Controllers
{
    public class NotifactionController : Controller
    {
        private readonly MyServiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public NotifactionController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = sign;
            _userManager = userManager;
        }


        // GET: Notifaction/ByRequest/5
        [Authorize("Basic")]
        
        public async Task<IActionResult> ByRequest(int? requestId)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            var identityUser = await _userManager.GetUserAsync(User);
            if (requestId == null)
            {
                return NotFound();
            }

            // Retrieve notifications for the specified request.
            var notifications = await _context.notifications
                .Where(n => n.UserId == identityUser.ToString())
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return NotFound();
            }

            return View(notifications);
        }

        // GET: Notifaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            // Optionally mark the notification as read when viewing details.
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
            }

            return View(notification);
        }

        // POST: Notifaction/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();

            // Redirect back to the notifications for the related request.
            return RedirectToAction(nameof(ByRequest), new { requestId = notification.requestId });
        }

        // POST: Notifaction/DeleteAll for a specific request
       
        public async Task<IActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if(await _userManager.IsInRoleAsync(identityUser, Helper.Roles.SuperAdmin.ToString()))
            {
                return RedirectToAction("Index", "Home");

            }

            var notifications = await _context.notifications
                .Where(n => n.UserId == identityUser.ToString())
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(notifications);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Retrieve notifications associated with the current user.
            var notifications = await _context.notifications
                .Where(n => n.UserId == currentUser.Id)
                .ToListAsync();

            if (!notifications.Any())
            {
                return Json(new { success = false, message = "No notifications found." });
            }

            _context.notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All notifications deleted successfully." });
        }

    }
}
