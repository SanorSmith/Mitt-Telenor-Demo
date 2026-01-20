using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billing");

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.PdfUrl).HasMaxLength(500);

            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Invoice)
                  .HasForeignKey(i => i.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Payments)
                  .WithOne(p => p.Invoice)
                  .HasForeignKey(p => p.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Quantity).IsRequired();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastFourDigits).HasMaxLength(4);
            entity.Property(e => e.ExpiryDate).HasMaxLength(7);
            entity.Property(e => e.IsDefault).IsRequired();
        });
    }
}
