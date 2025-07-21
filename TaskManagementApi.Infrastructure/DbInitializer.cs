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
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

                try
                {
                    await context.Database.MigrateAsync();

                    // Seed Roles
                    string[] roleNames = { "Admin", "User", "Developer", "QA", "TeamLead", "ProjectManager" };
                    foreach (var roleName in roleNames)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                            logger.LogInformation($"Role '{roleName}' created.");
                        }
                    }

                    // Seed Users
                    User? adminUser = await userManager.FindByEmailAsync("admin@softwarehouse.com");
                    User? devUser1 = await userManager.FindByEmailAsync("dev1@softwarehouse.com");
                    User? devUser2 = await userManager.FindByEmailAsync("dev2@softwarehouse.com");
                    User? qaUser1 = await userManager.FindByEmailAsync("qa1@softwarehouse.com");
                    User? teamLeadUser = await userManager.FindByEmailAsync("teamlead@softwarehouse.com");
                    User? pmUser = await userManager.FindByEmailAsync("pm@softwarehouse.com");

                    if (adminUser == null)
                    {
                        adminUser = new User { UserName = "admin@softwarehouse.com", Email = "admin@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(adminUser, "AdminPass123!");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Admin user seeded.");
                    }
                    if (devUser1 == null)
                    {
                        devUser1 = new User { UserName = "dev1@softwarehouse.com", Email = "dev1@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(devUser1, "DevPass123!");
                        await userManager.AddToRoleAsync(devUser1, "Developer");
                        logger.LogInformation("Developer dev1@softwarehouse.com seeded.");
                    }
                    if (devUser2 == null)
                    {
                        devUser2 = new User { UserName = "dev2@softwarehouse.com", Email = "dev2@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(devUser2, "DevPass123!");
                        await userManager.AddToRoleAsync(devUser2, "Developer");
                        logger.LogInformation("Developer dev2@softwarehouse.com seeded.");
                    }
                    if (qaUser1 == null)
                    {
                        qaUser1 = new User { UserName = "qa1@softwarehouse.com", Email = "qa1@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(qaUser1, "QAPass123!");
                        await userManager.AddToRoleAsync(qaUser1, "QA");
                        logger.LogInformation("QA qa1@softwarehouse.com seeded.");
                    }
                    if (teamLeadUser == null)
                    {
                        teamLeadUser = new User { UserName = "teamlead@softwarehouse.com", Email = "teamlead@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(teamLeadUser, "TLPass123!");
                        await userManager.AddToRoleAsync(teamLeadUser, "TeamLead");
                        logger.LogInformation("TeamLead teamlead@softwarehouse.com seeded.");
                    }
                    if (pmUser == null)
                    {
                        pmUser = new User { UserName = "pm@softwarehouse.com", Email = "pm@softwarehouse.com", EmailConfirmed = true };
                        await userManager.CreateAsync(pmUser, "PMPass123!");
                        await userManager.AddToRoleAsync(pmUser, "ProjectManager");
                        logger.LogInformation("ProjectManager pm@softwarehouse.com seeded.");
                    }

                    // Ensure all users are loaded after creation
                    adminUser = await userManager.FindByEmailAsync("admin@softwarehouse.com");
                    devUser1 = await userManager.FindByEmailAsync("dev1@softwarehouse.com");
                    devUser2 = await userManager.FindByEmailAsync("dev2@softwarehouse.com");
                    qaUser1 = await userManager.FindByEmailAsync("qa1@softwarehouse.com");
                    teamLeadUser = await userManager.FindByEmailAsync("teamlead@softwarehouse.com");
                    pmUser = await userManager.FindByEmailAsync("pm@softwarehouse.com");


                    // Seed Tasks and Subtasks
                    if (!context.TaskItems.Any() || !context.SubTaskItems.Any())
                    {
                        logger.LogInformation("Seeding default tasks and subtasks for software house scenario...");

                        if (adminUser != null && devUser1 != null && devUser2 != null && qaUser1 != null && teamLeadUser != null && pmUser != null)
                        {
                            // Tasks assigned by Admin
                            var adminAssignedTask1 = new TaskItem
                            {
                                Title = "Implement User Authentication Module",
                                Description = "Develop and integrate JWT-based authentication for the new web application.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(14),
                                UserId = devUser1.Id, // Assigned to Developer 1
                                AssignedByUserId = adminUser.Id, // Assigned by Admin
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddDays(13)
                            };

                            var adminAssignedTask2 = new TaskItem
                            {
                                Title = "Design Database Schema for Project X",
                                Description = "Create ER diagrams and SQL scripts for the new project's database.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(7),
                                UserId = teamLeadUser.Id, // Assigned to Team Lead
                                AssignedByUserId = adminUser.Id, // Assigned by Admin
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddDays(6)
                            };

                            // Tasks assigned by Team Lead
                            var tlAssignedTask1 = new TaskItem
                            {
                                Title = "Develop API Endpoint for Task Creation",
                                Description = "Create a RESTful API endpoint for adding new tasks to the system.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(5),
                                UserId = devUser2.Id, // Assigned to Developer 2
                                AssignedByUserId = teamLeadUser.Id, // Assigned by Team Lead
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddDays(4)
                            };

                            var tlAssignedTask2 = new TaskItem
                            {
                                Title = "Write Unit Tests for Task Service",
                                Description = "Develop comprehensive unit tests for the core task management business logic.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(3),
                                UserId = qaUser1.Id, // Assigned to QA 1
                                AssignedByUserId = teamLeadUser.Id, // Assigned by Team Lead
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddDays(2)
                            };

                            // Self-assigned tasks
                            var dev1SelfTask = new TaskItem
                            {
                                Title = "Refactor Old Legacy Code",
                                Description = "Improve readability and maintainability of existing codebase.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(20),
                                UserId = devUser1.Id, // Self-assigned
                                AssignedByUserId = null,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = false
                            };

                            var qa1SelfTask = new TaskItem
                            {
                                Title = "Perform Regression Testing on Build 1.0.1",
                                Description = "Execute full regression test suite on the latest deployment.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(2),
                                UserId = qaUser1.Id, // Self-assigned
                                AssignedByUserId = null,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddDays(1).AddHours(10) // Tomorrow at 10 AM
                            };

                            var pmSelfTask = new TaskItem
                            {
                                Title = "Prepare Project Status Report for Stakeholders",
                                Description = "Compile progress, risks, and next steps for weekly meeting.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(1),
                                UserId = pmUser.Id, // Self-assigned
                                AssignedByUserId = null,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                IsNotificationEnabled = true,
                                NotificationDateTime = DateTime.UtcNow.AddHours(2) // In 2 hours
                            };

                            context.TaskItems.AddRange(adminAssignedTask1, adminAssignedTask2, tlAssignedTask1, tlAssignedTask2, dev1SelfTask, qa1SelfTask, pmSelfTask);
                            await context.SaveChangesAsync(); // Save tasks to get their IDs

                            // Seed Subtasks
                            var subTask1_1 = new SubTaskItem
                            {
                                Title = "Implement JWT Token Generation",
                                Description = "Write code for generating JWT tokens upon successful login.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(7),
                                ParentTaskId = adminAssignedTask1.Id,
                                UserId = devUser1.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            var subTask1_2 = new SubTaskItem
                            {
                                Title = "Integrate Authentication Middleware",
                                Description = "Configure ASP.NET Core middleware to validate incoming JWT tokens.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(10),
                                ParentTaskId = adminAssignedTask1.Id,
                                UserId = devUser1.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            var subTask3_1 = new SubTaskItem
                            {
                                Title = "Define Task Model in C#",
                                Description = "Create the TaskItem entity class with properties and data annotations.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(2),
                                ParentTaskId = tlAssignedTask1.Id,
                                UserId = devUser2.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            var subTask3_2 = new SubTaskItem
                            {
                                Title = "Implement Task Repository Methods",
                                Description = "Write CRUD operations for tasks in the repository layer.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(3),
                                ParentTaskId = tlAssignedTask1.Id,
                                UserId = devUser2.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            var subTask4_1 = new SubTaskItem
                            {
                                Title = "Create Test Cases for Task Service",
                                Description = "Develop a set of test cases covering all scenarios for task business logic.",
                                IsCompleted = false,
                                DueDate = DateTime.UtcNow.AddDays(1),
                                ParentTaskId = tlAssignedTask2.Id,
                                UserId = qaUser1.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            context.SubTaskItems.AddRange(subTask1_1, subTask1_2, subTask3_1, subTask3_2, subTask4_1);
                            await context.SaveChangesAsync();

                            logger.LogInformation("Software house tasks and subtasks seeded.");
                        }
                        else
                        {
                            logger.LogWarning("One or more users not found, skipping detailed task and subtask seeding.");
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

