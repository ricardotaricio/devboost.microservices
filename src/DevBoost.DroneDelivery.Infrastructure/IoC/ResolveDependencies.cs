using DevBoost.DroneDelivery.Application.Services;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using DevBoost.DroneDelivery.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevBoost.DroneDelivery.CrossCutting.IOC
{
    public static class ResolveDependencies
    {
        public static IServiceCollection Register(this IServiceCollection services, IConfiguration  configuration)
        {

            services.AddTransient<DCDroneDelivery>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IDroneService, DroneService>();
            services.AddScoped<IDroneItinerarioService, DroneItinerarioService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IDroneItinerarioRepository, DroneItinerarioRepository>();
            services.AddScoped<IDroneRepository, DroneRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddDbContext<DCDroneDelivery>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly("DroneDelivery"));
            });

            return services;
        }
    }
}
