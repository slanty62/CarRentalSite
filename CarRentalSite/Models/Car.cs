using System.ComponentModel.DataAnnotations;

namespace CarRentalSite.Models;

public class Car
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Марка")]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Модель")]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Год выпуска")]
    public int Year { get; set; }

    [Display(Name = "Цена за день (₽)")]
    [Range(500, 50000)]
    public decimal PricePerDay { get; set; }

    [Display(Name = "Доступна")]
    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Изображение")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Описание")]
    public string? Description { get; set; }
}