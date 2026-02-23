using InvoiceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Data;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
        : base(options)
    { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(x => x.Id);
            u.Property(x => x.Email).IsRequired().HasMaxLength(200);
            u.Property(x => x.PasswordHash).IsRequired();
            u.Property(x => x.CreatedAt).IsRequired();
            u.Property(x => x.UpdatedAt).IsRequired();
            u.HasMany(x => x.RefreshTokens).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Token).IsRequired();
            r.Property(x => x.Expires).IsRequired();
            r.Property(x => x.CreatedAt).IsRequired();
        });

        // Customer 
        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(x => x.Id);
            c.Property(x => x.Name).IsRequired().HasMaxLength(200);
            c.Property(x => x.Address).HasMaxLength(500);
            c.Property(x => x.Email).IsRequired().HasMaxLength(200);
            c.Property(x => x.PhoneNumber).HasMaxLength(50);
            c.Property(x => x.CreatedAt).IsRequired();
            c.Property(x => x.UpdatedAt).IsRequired();
            c.HasQueryFilter(x => x.DeletedAt == null);

        });

        // Invoice, InvoiceRow 
        modelBuilder.Entity<Invoice>(i =>
        {
            i.HasKey(x => x.Id);
            i.Property(x => x.StartDate).IsRequired();
            i.Property(x => x.EndDate).IsRequired();
            i.Property(x => x.Comment).HasMaxLength(1000);
            i.Property(x => x.Status).IsRequired();
            i.Property(x => x.CreatedAt).IsRequired();
            i.Property(x => x.UpdatedAt).IsRequired();
            i.Property(x => x.TotalSum).IsRequired().HasColumnType("decimal(18,2)");

            i.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            i.HasMany(x => x.Rows).WithOne().HasForeignKey(r => r.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            i.HasQueryFilter(x => x.DeletedAt == null);
        });

        modelBuilder.Entity<InvoiceRow>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Service).IsRequired().HasMaxLength(300);
            r.Property(x => x.Quantity).IsRequired().HasColumnType("decimal(18,4)");
            r.Property(x => x.Rate).IsRequired().HasColumnType("decimal(18,4)");
            r.Property(x => x.Sum).IsRequired().HasColumnType("decimal(18,2)");
        });
    }

    // Centralized CreatedAt/UpdatedAt handling
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var propCreated = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
            var propUpdated = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");

            if (entry.State == EntityState.Added)
            {
                if (propCreated != null && (propCreated.CurrentValue == null || (DateTimeOffset)propCreated.CurrentValue == default))
                    propCreated.CurrentValue = now;
                if (propUpdated != null)
                    propUpdated.CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                if (propUpdated != null)
                    propUpdated.CurrentValue = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
