using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSite.Data;
using CarRentalSite.Models;

namespace CarRentalSite.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Берём только доступные автомобили
        var cars = await _context.Cars.Where(c => c.IsAvailable).ToListAsync();
        return View(cars);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}