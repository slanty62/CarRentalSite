using System.ComponentModel.DataAnnotations;

namespace CarRentalSite.Models.ViewModels;

public class RentCarViewModel
{
    public int CarId { get; set; }
    public string CarName { get; set; } = string.Empty;
    public decimal PricePerDay { get; set; }

    [Required(ErrorMessage = "Укажите дату начала")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Укажите дату окончания")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
}