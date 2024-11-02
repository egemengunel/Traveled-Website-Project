// Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using Travel_App.Data;
using Travel_App.Models;
using System.Linq;
using System.Collections.Generic;

namespace Travel_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var recentPlaces = _context.Places
                                        .OrderByDescending(p => p.Id)
                                        .Take(6)
                                        .ToList();
            ViewBag.RecentPlaces = recentPlaces;

            var groupedPlaces = _context.Places
                                         .GroupBy(p => p.Continent)
                                         .Select(g => new GroupedPlaceViewModel
                                         {
                                             Continent = g.Key,
                                             Places = g.ToList()
                                         })
                                         .ToList();
            ViewBag.GroupedPlaces = groupedPlaces;

            return View();
        }
    }
}
