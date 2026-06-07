using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalSite.Models;

namespace CarRentalSite.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Начальное наполнение (seed) – несколько автомобилей
        builder.Entity<Car>().HasData(
            new Car { Id = 1, Brand = "Toyota", Model = "Camry", Year = 2022, PricePerDay = 5000, IsAvailable = true, ImageUrl = "/images/camry.jpg", Description = "Надёжный седан для города и трассы" },
            new Car { Id = 2, Brand = "BMW", Model = "X5", Year = 2023, PricePerDay = 15000, IsAvailable = true, ImageUrl = "/images/bmw-x5.jpg", Description = "Премиум внедорожник" },
            new Car { Id = 3, Brand = "Hyundai", Model = "Solaris", Year = 2021, PricePerDay = 2200, IsAvailable = true, ImageUrl = "/images/solaris.jpg", Description = "Экономичный и манёвренный" },
            new Car { Id = 4, Brand = "Volkswagen", Model = "Polo", Year = 2015, PricePerDay = 7777, IsAvailable = true, ImageUrl = "/images/polo.jpg", Description = "Без комментариев.." },
            new Car { Id = 5, Brand = "Lada", Model = "Priora", Year = 2014, PricePerDay = 228, IsAvailable = true, ImageUrl = "/images/priora.jpg", Description = "Машина главного бандита Грэса" },
            new Car { Id = 6, Brand = "Lada", Model = "2115", Year = 2007, PricePerDay = 0, IsAvailable = true, ImageUrl = "/images/lada.jpg", Description = "Самая проблемная" },
            new Car { Id = 7, Brand = "Lamborghini", Model = "Huracan", Year = 2017, PricePerDay = 119000, IsAvailable = true, ImageUrl = "/images/lamba.jpg", Description = "Настоящий ураган" },
            new Car { Id = 8, Brand = "McLaren", Model = "720S Spider", Year = 2019, PricePerDay = 100000, IsAvailable = true, ImageUrl = "/images/mclaren.jpg", Description = "Открытая версия суперкара с жёсткой складной крышей (RHT) и карбоновой основой Monocage" },
            new Car { Id = 9, Brand = "Mercedes-Benz", Model = "S Class 222", Year = 2016, PricePerDay = 30000, IsAvailable = true, ImageUrl = "/images/mers.jpg", Description = "Автомобиль бизнес-класса - это хороший способ не только обозначить статус человека, но и обеспечить себе высокий уровень комфорта и удобства от поездки" },
            new Car { Id = 10, Brand = "Ford", Model = "Mustang", Year = 2018, PricePerDay = 20000, IsAvailable = true, ImageUrl = "/images/mustang.jpg", Description = "Автомобиль оснащен двигателем 2.3 EcoBoost мощностью 317 лошадиных сил" },
            new Car { Id = 11, Brand = "Porsche ", Model = "911 Carrera 4S", Year = 2013, PricePerDay = 45000, IsAvailable = true, ImageUrl = "/images/porshe.jpg", Description = "400 л.с , мягкая крыша, красный кожаный салон" },
            new Car { Id = 12, Brand = "Ferrari", Model = "Portofino", Year = 2017, PricePerDay = 85990, IsAvailable = true, ImageUrl = "/images/fera.jpg", Description = "Ferrari Portofino — идеальное воплощение итальянского Gran Turismo" }
        );
    }
}