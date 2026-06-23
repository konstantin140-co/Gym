using Gym.Data.Constants;
using Gym.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym.Data.Context;

public class AppDbContext : DbContext
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=gym.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Client)
            .WithMany(c => c.Subscriptions)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Client)
            .WithMany(c => c.Visits)
            .HasForeignKey(v => v.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Subscription)
            .WithMany(s => s.Visits)
            .HasForeignKey(v => v.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var today = DateTime.Today;

        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, LastName = "Иванов", FirstName = "Пётр", Phone = "+7 (900) 111-11-11", BirthDate = new DateTime(1995, 3, 12), RegistrationDate = new DateTime(2024, 1, 10) },
            new Client { Id = 2, LastName = "Петрова", FirstName = "Анна", Phone = "+7 (900) 222-22-22", BirthDate = new DateTime(1998, 7, 25), RegistrationDate = new DateTime(2024, 2, 5) },
            new Client { Id = 3, LastName = "Сидоров", FirstName = "Олег", Phone = "+7 (900) 333-33-33", BirthDate = new DateTime(1990, 11, 8), RegistrationDate = new DateTime(2023, 6, 15) },
            new Client { Id = 4, LastName = "Козлова", FirstName = "Мария", Phone = "+7 (900) 444-44-44", BirthDate = new DateTime(2000, 1, 30), RegistrationDate = new DateTime(2025, 1, 20) },
            new Client { Id = 5, LastName = "Новиков", FirstName = "Дмитрий", Phone = "+7 (900) 555-55-55", BirthDate = new DateTime(1988, 9, 14), RegistrationDate = new DateTime(2023, 11, 1) }
        );

        modelBuilder.Entity<Subscription>().HasData(
            new Subscription { Id = 1, ClientId = 1, Type = SubscriptionTypes.Month, StartDate = today.AddDays(-20), EndDate = today.AddDays(10), Price = 2500, Status = SubscriptionStatuses.Active },
            new Subscription { Id = 2, ClientId = 2, Type = SubscriptionTypes.Quarter, StartDate = today.AddDays(-60), EndDate = today.AddDays(30), Price = 6500, Status = SubscriptionStatuses.Active },
            new Subscription { Id = 3, ClientId = 3, Type = SubscriptionTypes.Month, StartDate = today.AddDays(-40), EndDate = today.AddDays(-10), Price = 2500, Status = SubscriptionStatuses.Expired },
            new Subscription { Id = 4, ClientId = 4, Type = SubscriptionTypes.Month, StartDate = today.AddDays(-25), EndDate = today.AddDays(5), Price = 2500, Status = SubscriptionStatuses.Active },
            new Subscription { Id = 5, ClientId = 5, Type = SubscriptionTypes.Year, StartDate = today.AddDays(-100), EndDate = today.AddDays(265), Price = 22000, Status = SubscriptionStatuses.Active }
        );

        modelBuilder.Entity<Visit>().HasData(
            new Visit { Id = 1, ClientId = 1, SubscriptionId = 1, DateTime = today.AddDays(-2).AddHours(10), ActivityType = ActivityTypes.Gym },
            new Visit { Id = 2, ClientId = 2, SubscriptionId = 2, DateTime = today.AddDays(-1).AddHours(18), ActivityType = ActivityTypes.Pool },
            new Visit { Id = 3, ClientId = 1, SubscriptionId = 1, DateTime = today.AddHours(9), ActivityType = ActivityTypes.Gym },
            new Visit { Id = 4, ClientId = 4, SubscriptionId = 4, DateTime = today.AddDays(-3).AddHours(12), ActivityType = ActivityTypes.Group },
            new Visit { Id = 5, ClientId = 5, SubscriptionId = 5, DateTime = today.AddDays(-5).AddHours(8), ActivityType = ActivityTypes.Yoga }
        );
    }
}
