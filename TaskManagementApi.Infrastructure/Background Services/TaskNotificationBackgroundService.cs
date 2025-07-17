using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Infrastructure.Background_Services
{
    public class TaskNotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<TaskNotificationBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _checkInterval;
        private readonly TimeSpan _notificationLeadTime;

        public TaskNotificationBackgroundService(
            ILogger<TaskNotificationBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            // Configure interval from appsettings.json
            var checkIntervalMinutes = configuration.GetValue<int>("NotificationService:CheckIntervalMinutes", 1);
            _checkInterval = TimeSpan.FromMinutes(checkIntervalMinutes);

            // Configure lead time from appsettings.json
            var notificationLeadTimeMinutes = configuration.GetValue<int>("NotificationService:NotificationLeadTimeMinutes", 5);
            _notificationLeadTime = TimeSpan.FromMinutes(notificationLeadTimeMinutes);

            _logger.LogInformation($"TaskNotificationBackgroundService initialized. Checking every {_checkInterval.TotalMinutes} minutes. Notifying tasks due within {_notificationLeadTime.TotalMinutes} minutes.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TaskNotificationBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("TaskNotificationBackgroundService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("TaskNotificationBackgroundService is checking for due tasks.");

                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var taskItemService = scope.ServiceProvider.GetRequiredService<ITaskItemService>();

                        // Get tasks that are due for notification using the new service method
                        var tasksToNotify = await taskItemService.GetTasksForNotificationAsync(_notificationLeadTime);

                        foreach (var task in tasksToNotify)
                        {
                            // Log the notification to the console
                            _logger.LogInformation($"Task Reminder: Task '{task.Title}' (ID: {task.Id}) for user '{task.User.UserName}' is due at {task.DueDate?.ToLocalTime()}. Notification set for {task.NotificationDateTime?.ToLocalTime()}.");

                            // Mark the task as notified to prevent repeat notifications
                            await taskItemService.MarkTaskAsNotifiedAsync(task.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking for task notifications.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("TaskNotificationBackgroundService has stopped.");
        }
    }
}
