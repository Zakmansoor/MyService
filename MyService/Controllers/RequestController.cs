using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using MyService.Resource;
using Microsoft.AspNetCore.Identity;

namespace MyService.Controllers
{
    public class RequestController : Controller
    {
        private readonly MyServiceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RequestController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = sign;
            _userManager = userManager;
        }

        // GET: Request/Create
        public async Task<IActionResult> SendRequest()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            
            

            var viewModel = new RequestViewModel
                {
                    OrderDate = DateTime.Today
                };

            if (_signInManager.IsSignedIn(User))
            {
                if (await _userManager.IsInRoleAsync(identityUser, Helper.Roles.SuperAdmin.ToString()))
                {
                    return RedirectToAction("Index", "Home");
                }

                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "Customer");
            }
        }

        // POST: Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(RequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve current user from Identity
                var identityUser = await _userManager.GetUserAsync(User);

                var service = await _context.services.FindAsync(viewModel.ServiceId);
                if (service == null)
                {
                    ModelState.AddModelError("", "The selected service does not exist.");
                    await PopulateDropdownsAsync(viewModel);
                    return View(viewModel);
                }

                var request = new Request
                {
                    UserId = identityUser.UserName,
                    ServiceId = viewModel.ServiceId,
                    OrderDate = viewModel.OrderDate,
                    Status = viewModel.Status,
                    Comment = viewModel.Comment,
                    Service = service, // Assign the loaded service entity
                    Mapping = new Mapping
                    {
                        Latitude = viewModel.Latitude,
                        Longitude = viewModel.Longitude
                    },
                };



                // Determine the price based on the service name.
                var paid = new Paid
                {
                    amount = GetPriceForService(service.ServiceID),
                    DateTime = DateTime.Now,
                    NameServece = service.Name,
                    Request = request
                };

                // Establish the relationship on the Request side.
                request.Paid = paid;

                // Add the request; cascade insertion of the related Paid entity is expected.
                _context.requests.Add(request);
                await _context.SaveChangesAsync();

                TempData["Success"] = ResourceWeb.lbSuccess;

                // You can add notification creation logic here if required.

                return RedirectToAction("Index", "Home");
            }
            else
            {
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }
        }

        /// <summary>
        /// Helper method to populate dropdown lists in the view model.
        /// </summary>
        private async Task PopulateDropdownsAsync(RequestViewModel viewModel)
        {
            viewModel.Services = await _context.services.ToListAsync();
        }

        /// <summary>
        /// Determines the predetermined price for a service based on its name.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <returns>The price for the service.</returns>
        private double GetPriceForService(int serviceId)
        {
            return serviceId switch
            {
                1 => 1200,    // كهرباء - Electrical
                2 => 1000,    // سباكة - Plumbing
                3 => 900,     // تكييف - AC Maintenance
                4 => 800,     // تنظيف - Cleaning
                5 => 750,     // دهان - Painting
                6 => 950,     // نجارة - Carpentry
                7 => 700,     // زجاج - Glass
                8 => 850,     // بلاط - Tiling
                9 => 600,     // مكافحة آفات - Pest Control
                10 => 650,   // حدادة - Metal Works
                _ => 0       // أي ID غير موجود
            };
        }
    }
}
