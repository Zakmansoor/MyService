using Microsoft.AspNetCore.Mvc;
using Domin.Entity;
using Infarstuructre.Data;
using System.Threading.Tasks;

namespace MyService.Controllers
{
    public class ContactController : Controller
    {
        private readonly MyServiceDbContext _context;

        public ContactController(MyServiceDbContext context)
        {
            _context = context;
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View(new Contact());
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact model)
        {
            if (ModelState.IsValid)
            {
                _context.contacts.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
