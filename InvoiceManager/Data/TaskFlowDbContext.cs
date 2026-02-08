using InvoiceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Data;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
        : base(options)
    {}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer
        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(x => x.Id);
            c.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
            c.Property(x => x.Address)
                .HasMaxLength(500);
            c.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);
            c.Property(x => x.PhoneNumber)
                .HasMaxLength(50);
            c.Property(x => x.CreatedAt)
                .IsRequired();
            c.Property(x => x.UpdatedAt) //add
                .IsRequired();

            // Soft delete filter
            c.HasQueryFilter(x => x.DeletedAt == null);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(i =>
        {
            i.HasKey(x => x.Id);
            i.Property(x => x.StartDate)
                .IsRequired();
            i.Property(x => x.EndDate)
                .IsRequired();
            i.Property(x => x.Comment)
                .HasMaxLength(1000);
            i.Property(x => x.Status)
                .IsRequired();
            i.Property(x => x.CreatedAt)
                .IsRequired();
            i.Property(x => x.UpdatedAt) //add
                .IsRequired();
            i.Property(x => x.TotalSum) //add
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            i.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            i.HasMany(x => x.Rows)
                .WithOne()
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            i.HasQueryFilter(x => x.DeletedAt == null);
        });

        // InvoiceRow
        modelBuilder.Entity<InvoiceRow>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Service)
                .IsRequired()
                .HasMaxLength(300);
            r.Property(x => x.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,4)");
            r.Property(x => x.Rate)
                .IsRequired()
                .HasColumnType("decimal(18,4)");
            r.Property(x => x.Sum) //add
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        });
    }

}
