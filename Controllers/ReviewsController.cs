using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Travel_App.Data;
using Travel_App.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Travel_App.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Details(int placeId)
        {
            _logger.LogInformation($"Fetching details for placeId: {placeId}");
            var place = _context.Places.FirstOrDefault(p => p.Id == placeId);
            if (place == null)
            {
                _logger.LogWarning($"No place found for placeId: {placeId}");
                return NotFound();
            }

            return View(place);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            return View(place);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Continent")] Place place, IFormFile ImageUrl)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            var existingPlace = await _context.Places.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (existingPlace == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageUrl != null && ImageUrl.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageUrl.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageUrl.CopyToAsync(fileStream);
                        }

                        place.ImageUrl = "/images/" + fileName; // Save relative path to the image
                    }
                    else
                    {
                        place.ImageUrl = existingPlace.ImageUrl; // Retain existing image
                    }

                    _context.Update(place);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { placeId = place.Id });
            }
            return View(place);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
