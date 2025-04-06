using System.Threading.Tasks;
using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyService.Controllers
{
    public class NoteController : Controller
    {
        private readonly MyServiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public NoteController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = sign;
            _userManager = userManager;
        }

        // GET: Note/Index
        public async Task<IActionResult> Index()
        {
            var notes = await _context.notes.ToListAsync();
            return View(notes);
        }

        // GET: Note/Create
        public async Task< IActionResult> Create()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (await _userManager.IsInRoleAsync(identityUser, Helper.Roles.SuperAdmin.ToString()))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Note/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(note model)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.notes.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Note created successfully.";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
