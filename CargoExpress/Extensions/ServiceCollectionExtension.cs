using CargoExpress.Areas.Admin.Contracts;
using CargoExpress.Areas.Admin.Services;
using CargoExpress.Core.Contracts;
using CargoExpress.Core.Services;
using CargoExpress.Infrastructure.Data;
using CargoExpress.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbRepository, ApplicationDbRepository>();
            services.AddScoped<ICargoService, CargoService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITruckService, TruckService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }
}
