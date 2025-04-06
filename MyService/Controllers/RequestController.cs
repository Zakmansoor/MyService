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
            if (await _userManager.IsInRoleAsync(identityUser, Helper.Roles.SuperAdmin.ToString()))
            {
              return RedirectToAction("Index", "Home");
            }
           
            var viewModel = new RequestViewModel
            {
                OrderDate = DateTime.Today
            };

            if (_signInManager.IsSignedIn(User))
            {
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
                    Service = service  // Assign the loaded service entity
                };

                // Determine the price based on the service name.
                var paid = new Paid
                {
                    amount = GetPriceForService(service.Name),
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
            viewModel.Customers = await _context.customers.ToListAsync();
            viewModel.Services = await _context.services.ToListAsync();
        }

        /// <summary>
        /// Determines the predetermined price for a service based on its name.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <returns>The price for the service.</returns>
        private double GetPriceForService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                return 0;

            // Convert to lower case for case-insensitive comparison.
            var lowerName = serviceName.ToLowerInvariant();

            // For example, if the service name is "سبالك" then the price is 1000.
            if (lowerName == "Plumbing")
                return 1000;
            // Add other service pricing rules here.
            else if (lowerName == "plumbing")
                return 750;
            else if (lowerName == "electrical")
                return 850;
            // Default price if no rule matches.
            else
                return 0;
        }
    }
}
