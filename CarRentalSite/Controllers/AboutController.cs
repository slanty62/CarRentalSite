using Microsoft.AspNetCore.Mvc;

namespace CarRentalSite.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}