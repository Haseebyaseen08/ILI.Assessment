using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }
        public DbSet<MonthlyUsageSummary> MonthlyUsageSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CustomerId).IsRequired(); // Make CustomerId required
                entity.HasIndex(e => e.Email).IsUnique();

                // Configure relationship with Customer - CASCADE DELETE
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Users)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade); // Delete users when customer is deleted
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).HasMaxLength(255);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.SubscriptionType).IsRequired();
            });

            // Configure ApiUsageLog entity
            modelBuilder.Entity<ApiUsageLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Timestamp).IsRequired();

                // Configure relationship with Customer
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.ApiUsageLogs)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship with User
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ApiUsageLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user if there are API usage logs
            });

            // Configure MonthlyUsageSummary entity
            modelBuilder.Entity<MonthlyUsageSummary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Property(e => e.Year).IsRequired();
                entity.Property(e => e.Month).IsRequired();
                entity.Property(e => e.SubscriptionType).IsRequired();
                entity.Property(e => e.TotalApiCalls).IsRequired();
                entity.Property(e => e.PricePerCall).HasPrecision(18, 4);
                entity.Property(e => e.TotalCost).HasPrecision(18, 4);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate).IsRequired();
                entity.Property(e => e.EndpointUsageJson).HasMaxLength(4000);

                // Configure relationship with Customer
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create unique index for Customer, Year, Month
                entity.HasIndex(e => new { e.CustomerId, e.Year, e.Month }).IsUnique();
            });
        }
    }
}