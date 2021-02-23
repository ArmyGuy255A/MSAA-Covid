using Microsoft.AspNetCore.Mvc;
using MSSA_Covid.Data;
using MSSA_Covid.Services;
using MSSA_Covid.ViewModels;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MSSA_Covid.Controllers
{
    [Route("Kiosk")]
    public class KioskController : Controller
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly DefaultDBContext _context;
        public KioskController (BlobStorageService blobStorageService, DefaultDBContext defaultDbContext)
        {
            _blobStorageService = blobStorageService;
            _context = defaultDbContext;
        }

        public async Task<IActionResult> Index(int id)
        {
            var model = _context.KioskConfigurations.Any();
            if (id == 0)
            {
                return View(await _context.KioskConfigurations.Include(f => f.KioskFiles).FirstOrDefaultAsync());
            } else
            {
                return View(await _context.KioskConfigurations.Include(f => f.KioskFiles).FirstOrDefaultAsync(o => o.Id == id));
            }            
        }

        [Route("Remote")]
        public IActionResult Remote()
        {
            ViewData["Title"] = "MSSA - Kiosk Remote Control";
            return View();
        }
    }
}
