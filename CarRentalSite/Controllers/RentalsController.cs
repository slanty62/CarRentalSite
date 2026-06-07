using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSite.Data;
using CarRentalSite.Models;
using CarRentalSite.Models.ViewModels;

namespace CarRentalSite.Controllers;

[Authorize]
public class RentalsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RentalsController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public RentalsController(ApplicationDbContext context, ILogger<RentalsController> logger, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    // Возвращает ID текущего пользователя (GUID), а не Email
    private async Task<string?> GetCurrentUserIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.Id;
    }

    // Мои бронирования
    public async Task<IActionResult> MyRentals()
    {
        var userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Пользователь не аутентифицирован при попытке просмотра бронирований");
            return Challenge();
        }

        var rentals = await _context.Rentals
            .Include(r => r.Car)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.BookingDate)
            .ToListAsync();
        return View(rentals);
    }

    // GET: Форма создания аренды
    public async Task<IActionResult> Create(int carId)
    {
        try
        {
            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                TempData["Error"] = "Автомобиль не найден";
                return RedirectToAction("Index", "Cars");
            }
            if (!car.IsAvailable)
            {
                TempData["Error"] = "Этот автомобиль уже недоступен";
                return RedirectToAction("Details", "Cars", new { id = carId });
            }

            var model = new RentCarViewModel
            {
                CarId = car.Id,
                CarName = $"{car.Brand} {car.Model}",
                PricePerDay = car.PricePerDay,
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(3)
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке формы бронирования для carId {CarId}", carId);
            TempData["Error"] = "Произошла ошибка. Попробуйте позже.";
            return RedirectToAction("Index", "Cars");
        }
    }

    // POST: Создание аренды
    [HttpPost]
    // [ValidateAntiForgeryToken] // Для fetch-запросов пока отключён
    public async Task<IActionResult> Create(RentCarViewModel model)
    {
        var userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        _logger.LogInformation("===> Начало POST Create. CarId={0}, StartDate={1}, EndDate={2}",
            model.CarId, model.StartDate, model.EndDate);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState невалиден. Ошибки: {Errors}",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(model);
        }

        try
        {
            var car = await _context.Cars.FindAsync(model.CarId);
            if (car == null || !car.IsAvailable)
            {
                ModelState.AddModelError("", "Автомобиль не найден или уже недоступен");
                return View(model);
            }

            var start = model.StartDate.Date;
            var end = model.EndDate.Date;

            if (start >= end)
            {
                ModelState.AddModelError("", "Дата окончания должна быть позже даты начала");
                return View(model);
            }
            if (start <= DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "Дата начала должна быть не ранее завтрашнего дня");
                return View(model);
            }

            var overlapping = await _context.Rentals.AnyAsync(r =>
                r.CarId == model.CarId &&
                r.Status == "Active" &&
                start < r.EndDate && end > r.StartDate);

            if (overlapping)
            {
                ModelState.AddModelError("", "Выбранные даты пересекаются с существующей арендой");
                return View(model);
            }

            int days = (int)(end - start).TotalDays;
            if (days <= 0) days = 1;
            var total = days * car.PricePerDay;

            var rental = new Rental
            {
                UserId = userId,   // теперь это GUID, а не email
                CarId = model.CarId,
                StartDate = start,
                EndDate = end,
                TotalPrice = total,
                BookingDate = DateTime.Now,
                Status = "Active"
            };

            _context.Rentals.Add(rental);
            _logger.LogInformation($"Бронирование создано: UserId={userId}, CarId={model.CarId}, Start={start}, End={end}, Total={total}");
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Cars", new { id = model.CarId, success = true });
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Ошибка БД при создании аренды. Внутреннее исключение: {Inner}", dbEx.InnerException?.Message);
            ModelState.AddModelError("", "Не удалось сохранить бронирование из-за проблем с базой данных.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неизвестная ошибка при создании аренды: {Message}", ex.Message);
            ModelState.AddModelError("", "Произошла внутренняя ошибка. Попробуйте позже.");
            return View(model);
        }
    }

    // Отмена бронирования
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var rental = await _context.Rentals
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (rental == null)
        {
            TempData["Error"] = "Бронирование не найдено";
            return RedirectToAction(nameof(MyRentals));
        }

        if (rental.Status != "Active")
        {
            TempData["Error"] = "Невозможно отменить уже завершённое или отменённое бронирование";
            return RedirectToAction(nameof(MyRentals));
        }

        rental.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Success"] = "Бронирование отменено";
        return RedirectToAction(nameof(MyRentals));
    }
}