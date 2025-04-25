using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Domin.Entity;
using Infarstuructre.IRepository;
using Infarstuructre.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;


namespace MyService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(CultureInfo.CurrentCulture.Name.StartsWith("ar"))
                {
                return RedirectToAction("Indexar", "Home");

            }

            return View();
        }public IActionResult Indexar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            var supportedCultures = new[] { "en-US", "ar-SA" };

            if (!supportedCultures.Contains(culture))
            {
                culture = "en-US";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                }
            );

            if (culture == "ar-SA")
            {
                return RedirectToAction("Indexar", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
 
    }
}
////WOOOOOOOOOOOOOOOOOOOOOOOOOO