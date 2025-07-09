using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Interface.IRepositories;
using TaskManagementApi.Core.Repositories;

namespace TaskManagementApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext with SQL Server.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register the UnitOfWork. It will manage the DbContext and repositories.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
