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

                    // Seed Roles
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
                    User? user1 = await userManager.FindByEmailAsync("user1@example.com");
                    User? user2 = await userManager.FindByEmailAsync("user2@example.com");

                    if (user1 == null)
                    {
                        user1 = new User { UserName = "user1@example.com", Email = "user1@example.com", EmailConfirmed = true };
                        await userManager.CreateAsync(user1, "Password123!");
                        await userManager.AddToRoleAsync(user1, "Admin");
                        logger.LogInformation("Admin user1@example.com seeded.");
                    }
                    if (user2 == null)
                    {
                        user2 = new User { UserName = "user2@example.com", Email = "user2@example.com", EmailConfirmed = true };
                        await userManager.CreateAsync(user2, "Password123!");
                        await userManager.AddToRoleAsync(user2, "User");
                        logger.LogInformation("Regular user2@example.com seeded.");
                    }

                    // Seed Tasks and Subtasks
                    if (!context.TaskItems.Any() || !context.SubTaskItems.Any())
                    {
                        logger.LogInformation("Seeding default tasks and subtasks...");

                        // Ensure users are loaded if they were just created
                        user1 = await userManager.FindByEmailAsync("user1@example.com");
                        user2 = await userManager.FindByEmailAsync("user2@example.com");

                        if (user1 != null && user2 != null)
                        {
                            var task1 = new TaskItem
                            {
                                Title = "Admin Assigned Task 1 (for user2)",
                                Description = "This task was assigned by the admin to user2.",
                                IsCompleted = false,
                                DueDate = null, // Admin sets to null
                                UserId = user2.Id, // Assigned to user2
                                AssignedByUserId = user1.Id, // Assigned by admin1
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = false, // Admin sets to false
                                NotificationDateTime = null // Admin sets to null
                            };

                            var task2 = new TaskItem
                            {
                                Title = "Admin Assigned Task 2 (for user2)",
                                Description = "Another task assigned by the admin to user2.",
                                IsCompleted = false,
                                DueDate = null,
                                UserId = user2.Id,
                                AssignedByUserId = user1.Id, // Assigned by admin1
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = false,
                                NotificationDateTime = null
                            };

                            var task3 = new TaskItem
                            {
                                Title = "User2's Self-Created Task",
                                Description = "This task was created directly by user2.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(7),
                                UserId = user2.Id,
                                AssignedByUserId = null, // Not assigned by an admin
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddMinutes(5)
                            };

                            var task4 = new TaskItem
                            {
                                Title = "Admin's Own Task",
                                Description = "Task created by the admin for themselves.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(10),
                                UserId = user1.Id, // Owned by admin
                                AssignedByUserId = user1.Id, // Created by admin for themselves
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddMinutes(1)
                            };

                            context.TaskItems.AddRange(task1, task2, task3, task4);
                            await context.SaveChangesAsync(); // Save tasks to get their IDs

                            // Seed Subtasks for task1 (owned by user2)
                            var subTask1_1 = new SubTaskItem
                            {
                                Title = "Subtask 1.1 for Task 1",
                                Description = "First subtask for Admin Assigned Task 1.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(1),
                                ParentTaskId = task1.Id,
                                UserId = user2.Id, // Owned by user2
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            var subTask1_2 = new SubTaskItem
                            {
                                Title = "Subtask 1.2 for Task 1",
                                Description = "Second subtask for Admin Assigned Task 1.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(3),
                                ParentTaskId = task1.Id,
                                UserId = user2.Id, // Owned by user2
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            // Seed Subtask for task3 (owned by user2)
                            var subTask3_1 = new SubTaskItem
                            {
                                Title = "Subtask 3.1 for User2's Task",
                                Description = "Subtask for user2's self-created task.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(8),
                                ParentTaskId = task3.Id,
                                UserId = user2.Id, // Owned by user2
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            context.SubTaskItems.AddRange(subTask1_1, subTask1_2, subTask3_1);
                            await context.SaveChangesAsync();

                            logger.LogInformation("Default tasks and subtasks seeded.");
                        }
                        else
                        {
                            logger.LogWarning("Users not found, skipping task and subtask seeding.");
                        }
                    }
                    else
                    {
                        logger.LogInformation("Database already contains tasks and subtasks. Skipping seeding.");
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

