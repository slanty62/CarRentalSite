using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarRentalSite.Models;

public class Rental
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    [Required]
    public int CarId { get; set; }
    public Car? Car { get; set; }

    [Display(Name = "Дата начала")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "Дата окончания")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Общая стоимость")]
    public decimal TotalPrice { get; set; }

    [Display(Name = "Дата бронирования")]
    public DateTime BookingDate { get; set; } = DateTime.Now;

    [Display(Name = "Статус")]
    public string Status { get; set; } = "Active"; // Active, Completed, Cancelled
}