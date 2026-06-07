using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSite.Data;
using CarRentalSite.Models;

namespace CarRentalSite.Controllers;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Список всех доступных авто
    public async Task<IActionResult> Index()
    {
        var cars = await _context.Cars.Where(c => c.IsAvailable).ToListAsync();
        return View(cars);
    }

    // Детали авто
    public async Task<IActionResult> Details(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(); // 404 страница
        }
        return View(car);
    }
}