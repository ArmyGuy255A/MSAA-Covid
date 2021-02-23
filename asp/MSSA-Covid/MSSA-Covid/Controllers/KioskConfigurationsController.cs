using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSSA_Covid.Data;
using MSSA_Covid.Data.Interfaces;
using MSSA_Covid.Data.Models;
using MSSA_Covid.Services;

namespace MSSA_Covid.Controllers
{
    [Route("Kiosk/Admin")]
    public class KioskConfigurationsController : Controller
    {
        private readonly DefaultDBContext _context;
        private readonly BlobStorageService _blobStorageService;
        public string BlobPath { get; set; }

        public KioskConfigurationsController(DefaultDBContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            BlobPath = "Kiosk";
        }

        // GET: KioskConfigurations
        public async Task<IActionResult> Index()
        {
            return View(await _context.KioskConfigurations.Include(f => f.KioskFiles).ToListAsync());
        }

        // GET: KioskConfigurations/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kioskConfiguration = await _context.KioskConfigurations
                .Include(f => f.KioskFiles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kioskConfiguration == null)
            {
                return NotFound();
            }

            return View(kioskConfiguration);
        }

        // GET: KioskConfigurations/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: KioskConfigurations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] KioskConfiguration kioskConfiguration, IFormFile RawFiles)
        {
            if (ModelState.IsValid)
            {
                await UploadFileAsync(RawFiles, kioskConfiguration);

                _context.Add(kioskConfiguration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kioskConfiguration);
        }

        

        // GET: KioskConfigurations/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kioskConfiguration = await _context.KioskConfigurations.Where(o => o.Id == id).Include(f => f.KioskFiles).FirstOrDefaultAsync();
            if (kioskConfiguration == null)
            {
                return NotFound();
            }
            return View(kioskConfiguration);
        }        

        // POST: KioskConfigurations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] KioskConfiguration kioskConfiguration, IFormFile RawFiles)
        {
            if (id != kioskConfiguration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    //verify that the file has changed
                    bool notChanged = await VerifyFileAsync(RawFiles, kioskConfiguration);
                    if (!notChanged)
                    {
                        //Delete the old file
                        await DeleteBlobAsync(RawFiles, kioskConfiguration);

                        //Then, upload a new one.
                        await UploadFileAsync(RawFiles, kioskConfiguration);
                    }

                    _context.Update(kioskConfiguration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KioskConfigurationExists(kioskConfiguration.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(kioskConfiguration);
        }

        // GET: KioskConfigurations/EditFile/5
        [Route("EditFile")]
        public async Task<IActionResult> EditFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kioskFile = await _context.KioskFiles.FindAsync(id);
            if (kioskFile == null)
            {
                return NotFound();
            }
            ViewData["KioskConfigurationId"] = new SelectList(_context.KioskConfigurations, "Id", "Name", kioskFile.KioskConfigurationId);
            return View(kioskFile);
        }

        // POST: KioskFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditFile")]
        public async Task<IActionResult> EditFile(int id, [Bind("Id,FileName,Title,Description,BlobUri,KioskConfigurationId")] KioskFile kioskFile)
        {
            if (id != kioskFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kioskFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KioskFileExists(kioskFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KioskConfigurationId"] = new SelectList(_context.KioskConfigurations, "Id", "Name", kioskFile.KioskConfigurationId);
            return View(kioskFile);
        }

        // GET: KioskConfigurations/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kioskConfiguration = await _context.KioskConfigurations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kioskConfiguration == null)
            {
                return NotFound();
            }

            return View(kioskConfiguration);
        }

        // POST: KioskConfigurations/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kioskConfiguration = await _context.KioskConfigurations.FindAsync(id);

            // Delete the blob files
            foreach (var item in kioskConfiguration.RawFiles)
            {
                await DeleteBlobAsync(item, kioskConfiguration);
            }

            _context.KioskConfigurations.Remove(kioskConfiguration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: KioskFiles/Delete/5
        [Route("DeleteFile")]
        public async Task<IActionResult> DeleteFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kioskFile = await _context.KioskFiles
                .Include(k => k.KioskConfiguration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kioskFile == null)
            {
                return NotFound();
            }

            return View(kioskFile);
        }

        // POST: KioskFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("DeleteFile")]
        public async Task<IActionResult> DeleteFileConfirmed(int id)
        {
            var kioskFile = await _context.KioskFiles.FindAsync(id);

            // Delete the blob files
            await DeleteBlobAsync(kioskFile);

            _context.KioskFiles.Remove(kioskFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KioskFileExists(int id)
        {
            return _context.KioskFiles.Any(e => e.Id == id);
        }

        private bool KioskConfigurationExists(int id)
        {
            return _context.KioskConfigurations.Any(e => e.Id == id);
        }

        private async Task<bool> UploadFileAsync(IFormFile file, KioskConfiguration kioskConfiguration)
        {
            if (null != file) 
            {
                KioskFile kFile = new KioskFile();
                kFile.FileName = file.FileName;
                kFile.BlobUri = await _blobStorageService.UploadFileToBlobAsync(file, GetBlobPath(kioskConfiguration));

                if (null == kioskConfiguration.KioskFiles)
                {
                    kioskConfiguration.KioskFiles = new List<KioskFile>();
                }

                kioskConfiguration.KioskFiles.Add(kFile);
                return true;
            }
            return false;
        }

        private async Task<bool> VerifyFileAsync(IFormFile file, KioskConfiguration kioskConfiguration)
        {
            if (null == file)
            {
                return true;
            } 
            else
            {
                return await _blobStorageService.VerifyBlobAsync(GetBlobPath(kioskConfiguration), file);
            }
        }

        private async Task<bool> DeleteBlobAsync(IFormFile file, KioskConfiguration kioskConfiguration)
        { 
            return await _blobStorageService.DeleteBlobAsync(String.Format("{0}/{1}", GetBlobPath(kioskConfiguration), file.FileName));
        }

        private async Task<bool> DeleteBlobAsync(KioskFile file)
        {
            return await _blobStorageService.DeleteBlobAsync(file);
        }

        private string GetBlobPath(KioskConfiguration kioskConfiguration)
        {
            return String.Format("{0}/{1}", BlobPath, kioskConfiguration.Id);
        }


    }
}
