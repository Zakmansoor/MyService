using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyService.Areas.WorkersEmp.Controllers
{
    [Area("WorkersEmp")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Denied()
        {
            return View();
        }
    }
}
