using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Infrastructure
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); // NEW: RoleManager
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

                try
                {
                    // Ensure the database is created and migrations are applied
                    await context.Database.MigrateAsync();

                    // Seed Roles (NEW)
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                        logger.LogInformation("Role 'Admin' created.");
                    }
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                        logger.LogInformation("Role 'User' created.");
                    }

                    // Seed Users
                    if (!userManager.Users.Any())
                    {
                        logger.LogInformation("Seeding default users...");

                        var user1 = new User { UserName = "user1@example.com", Email = "user1@example.com", EmailConfirmed = true };
                        var user2 = new User { UserName = "user2@example.com", Email = "user2@example.com", EmailConfirmed = true };

                        await userManager.CreateAsync(user1, "Password123!");
                        await userManager.CreateAsync(user2, "Password123!");

                        // Assign roles to users (NEW)
                        await userManager.AddToRoleAsync(user1, "Admin");
                        await userManager.AddToRoleAsync(user2, "User");

                        logger.LogInformation("Default users and roles seeded.");
                    }

                    // Seed Tasks
                    if (!context.TaskItems.Any())
                    {
                        logger.LogInformation("Seeding default tasks...");

                        var user1 = await userManager.FindByEmailAsync("user1@example.com");
                        var user2 = await userManager.FindByEmailAsync("user2@example.com");

                        if (user1 != null && user2 != null)
                        {
                            context.TaskItems.AddRange(
                                new TaskItem
                                {
                                    Title = "Buy groceries",
                                    Description = "Milk, Eggs, Bread, Cheese",
                                    IsCompleted = false,
                                    DueDate = DateTime.UtcNow.AddDays(2),
                                    UserId = user1.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                },
                                new TaskItem
                                {
                                    Title = "Finish report",
                                    Description = "Complete the quarterly sales report for Q2",
                                    IsCompleted = false,
                                    DueDate = DateTime.UtcNow.AddDays(5),
                                    UserId = user1.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                },
                                new TaskItem
                                {
                                    Title = "Call plumber",
                                    Description = "Leaky faucet in the bathroom",
                                    IsCompleted = true,
                                    DueDate = DateTime.UtcNow.AddDays(-1),
                                    UserId = user1.Id,
                                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                                },
                                new TaskItem
                                {
                                    Title = "Plan vacation",
                                    Description = "Research destinations and book flights",
                                    IsCompleted = false,
                                    DueDate = DateTime.UtcNow.AddDays(30),
                                    UserId = user2.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                },
                                new TaskItem
                                {
                                    Title = "Workout",
                                    Description = "Go to the gym for an hour",
                                    IsCompleted = false,
                                    DueDate = DateTime.UtcNow.AddDays(1),
                                    UserId = user2.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                }
                            );
                            await context.SaveChangesAsync();
                            logger.LogInformation("Default tasks seeded.");
                        }
                        else
                        {
                            logger.LogWarning("Users not found, skipping task seeding.");
                        }
                    }
                    else
                    {
                        logger.LogInformation("Database already contains tasks. Skipping task seeding.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
