using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Enums;

namespace Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DBContext>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Seed Customers first (since Users reference Customers)
                await SeedCustomersAsync(context, logger);

                // Seed Users with Customer assignments
                await SeedUsersAsync(context, logger);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedCustomersAsync(DBContext context, ILogger logger)
        {
            if (await context.Customers.AnyAsync())
            {
                logger.LogInformation("Customers already exist. Skipping customer seeding.");
                return;
            }

            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerName = "Acme Corporation",
                    CompanyName = "Acme Corp",
                    ContactEmail = "contact@acme.com",
                    ContactPhone = "+1-555-0001",
                    CreatedDate = DateTime.UtcNow.AddDays(-30),
                    IsActive = true,
                    SubscriptionType = SubscriptionType.Pro
                },
                new Customer
                {
                    CustomerName = "Tech Solutions Ltd",
                    CompanyName = "Tech Solutions",
                    ContactEmail = "info@techsolutions.com",
                    ContactPhone = "+1-555-0002",
                    CreatedDate = DateTime.UtcNow.AddDays(-25),
                    IsActive = true,
                    SubscriptionType = SubscriptionType.Free
                },
                new Customer
                {
                    CustomerName = "Global Enterprises",
                    CompanyName = "Global Ent",
                    ContactEmail = "admin@globalent.com",
                    ContactPhone = "+1-555-0003",
                    CreatedDate = DateTime.UtcNow.AddDays(-20),
                    IsActive = true,
                    SubscriptionType = SubscriptionType.Pro
                },
                new Customer
                {
                    CustomerName = "Startup Inc",
                    CompanyName = "Startup Inc",
                    ContactEmail = "hello@startup.com",
                    ContactPhone = "+1-555-0004",
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    IsActive = false,
                    SubscriptionType = SubscriptionType.Free
                },
                new Customer
                {
                    CustomerName = "Innovation Labs",
                    CompanyName = "Innovation Labs",
                    ContactEmail = "contact@innovationlabs.com",
                    ContactPhone = "+1-555-0005",
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    IsActive = true,
                    SubscriptionType = SubscriptionType.Pro
                }
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {customers.Count} customers.");
        }

        private static async Task SeedUsersAsync(DBContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist. Skipping user seeding.");
                return;
            }

            // Get customers to assign to users
            var customers = await context.Customers.ToListAsync();

            if (!customers.Any())
            {
                logger.LogError("No customers found. Cannot create users without customers since CustomerId is required.");
                throw new InvalidOperationException("Cannot seed users without customers. Please ensure customers are seeded first.");
            }

            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password123!", workFactor: 12),
                    CustomerId = customers[0].Id // Acme Corporation - REQUIRED
                },
                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("SecurePass456!", workFactor: 12),
                    CustomerId = customers[0].Id // Acme Corporation - REQUIRED
                },
                new User
                {
                    FirstName = "Mike",
                    LastName = "Johnson",
                    Email = "mike.johnson@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("StrongPass789!", workFactor: 12),
                    CustomerId = customers[1].Id // Tech Solutions Ltd - REQUIRED
                },
                new User
                {
                    FirstName = "Sarah",
                    LastName = "Williams",
                    Email = "sarah.williams@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("MyPassword101!", workFactor: 12),
                    CustomerId = customers[2].Id // Global Enterprises - REQUIRED
                },
                new User
                {
                    FirstName = "David",
                    LastName = "Brown",
                    Email = "david.brown@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("AdminPass202!", workFactor: 12),
                    CustomerId = customers[4].Id // Innovation Labs - REQUIRED
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {users.Count} users with hashed passwords and required customer assignments.");
        }
    }
}