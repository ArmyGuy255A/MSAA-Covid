using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSSA_Covid.Controllers
{
    [Route("Kiosk")]
    public class KioskController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "MSSA - Kiosk Page";
            return View();
        }

        [Route("Remote")]
        public IActionResult Remote()
        {
            ViewData["Title"] = "MSSA - Kiosk Remote Control";
            return View();
        }
    }
}
