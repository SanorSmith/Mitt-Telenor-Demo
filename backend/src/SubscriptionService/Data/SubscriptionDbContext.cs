using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;

namespace SubscriptionService.Data;

public class SubscriptionDbContext : DbContext
{
    public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) { }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AddOn> AddOns { get; set; }
    public DbSet<SubscriptionAddOn> SubscriptionAddOns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("subscriptions");

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.DataAllowanceGB).IsRequired();
            entity.Property(e => e.VoiceMinutes).IsRequired();
            entity.Property(e => e.SmsCount).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.BillingCycle).IsRequired().HasMaxLength(20);

            entity.HasOne(e => e.Plan)
                  .WithMany()
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SubscriptionAddOns)
                  .WithOne(sa => sa.Subscription)
                  .HasForeignKey(sa => sa.SubscriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AddOn>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsActive).IsRequired();
        });

        modelBuilder.Entity<SubscriptionAddOn>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.AddOn)
                  .WithMany()
                  .HasForeignKey(e => e.AddOnId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var plans = new[]
        {
            new Plan
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Basic 5GB",
                Description = "Perfect for light users",
                Price = 199.00m,
                DataAllowanceGB = 5,
                VoiceMinutes = 200,
                SmsCount = 100,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Standard 10GB",
                Description = "Great for everyday use",
                Price = 299.00m,
                DataAllowanceGB = 10,
                VoiceMinutes = 500,
                SmsCount = 200,
                IsActive = true
            },
            new Plan
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Premium 20GB",
                Description = "Unlimited possibilities",
                Price = 499.00m,
                DataAllowanceGB = 20,
                VoiceMinutes = 1000,
                SmsCount = 500,
                IsActive = true
            }
        };

        var addOns = new[]
        {
            new AddOn
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Extra 5GB Data",
                Description = "Additional 5GB data allowance",
                Price = 99.00m,
                Type = "Data",
                Value = 5,
                IsActive = true
            },
            new AddOn
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "International Calls",
                Description = "100 minutes international calls",
                Price = 149.00m,
                Type = "Voice",
                Value = 100,
                IsActive = true
            }
        };

        modelBuilder.Entity<Plan>().HasData(plans);
        modelBuilder.Entity<AddOn>().HasData(addOns);
    }
}
