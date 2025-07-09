using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Services;


namespace TaskManagementApi.Core
{
    public static class DependencyInjection
    {
        // Extension method to add Core layer services to the IServiceCollection.
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Register individual services
            services.AddScoped<ITaskItemService, TaskItemService>();

            // Register the UnitOfService, which encapsulates all other services
            services.AddScoped<IUnitOfService, UnitOfService>();

            return services;
        }
    }
}
