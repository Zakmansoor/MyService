using Domin.Entity;
using Infarstuructre.Data;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
      
            private readonly MyServiceDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;

            public HomeController(MyServiceDbContext context, SignInManager<ApplicationUser> sign, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _signInManager = sign;
                _userManager = userManager;
            }


            public async Task<IActionResult> IndexAsync()
        {
            dashboardadmin dashboardadmin = new dashboardadmin()
            {
                services = _context.services.ToList(),
                requests = _context.requests.ToList(),
                employees = _context.employees.ToList(),
                categories = _context.Categories.ToList(),
                historypaids = _context.historypaids.ToList(),
                ActiveUsersCount = _context.Users.Count(u => u.ActiveUser==true),
                InactiveUsersCount = _context.Users.Count(u => u.ActiveUser==false),
                BasicUserCount = (await _userManager.GetUsersInRoleAsync(@Helper.Roles.Basic.ToString())).Count()


            };
            return View(dashboardadmin);
        }
        public IActionResult Denied()
        {
            return View();
        }
    }
}
