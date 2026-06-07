using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSite.Data;
using CarRentalSite.Models;

namespace CarRentalSite.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class CarsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CarsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/carsapi – получить все машины
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        return await _context.Cars.ToListAsync();
    }

    // GET: api/carsapi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return car;
    }

    // POST: api/carsapi/rent – создать аренду (JSON)
    [HttpPost("rent")]
    public async Task<IActionResult> RentCar([FromBody] ApiRentalRequest request)
    {
        var car = await _context.Cars.FindAsync(request.CarId);
        if (car == null || !car.IsAvailable)
            return BadRequest("Car not available");

        // Здесь нужна проверка пользователя через токен (для простоты – userId из тела)
        var rental = new Rental
        {
            UserId = request.UserId,
            CarId = request.CarId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPrice = (request.EndDate - request.StartDate).Days * car.PricePerDay,
            BookingDate = DateTime.UtcNow,
            Status = "Active"
        };
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        return Ok(rental);
    }
}

// DTO для API
public class ApiRentalRequest
{
    public string UserId { get; set; } = string.Empty;
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}