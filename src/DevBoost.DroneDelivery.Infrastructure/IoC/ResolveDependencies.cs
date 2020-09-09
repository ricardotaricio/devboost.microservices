using AutoMapper;
using DevBoost.DroneDelivery.Application.Bus;
using DevBoost.DroneDelivery.Application.Services;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Infrastructure.AutoMapper;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using DevBoost.DroneDelivery.Infrastructure.Data.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.CrossCutting.IOC
{
    [ExcludeFromCodeCoverage]

    public static class ResolveDependencies
    {
        public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
        {
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
          
            

            TokenGenerator.TokenConfig = configuration.GetSection("Token").Get<Token>();

            var assembly = AppDomain.CurrentDomain.Load("DevBoost.DroneDelivery.Pagamento.Application");
            services.AddMediatR(assembly);
            services.AddTransient<IMediatrHandler, MediatrHandler>();

            
            services.AddAutoMapper(typeof(DtoToCommandMappingProfile), typeof(CommandToDomainMappingProfile), typeof(ViewModelToCommandMappingProfile), typeof(DomainToDtoMappingProfile));

            

            services.AddDbContext<DCDroneDelivery>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            


            return services;
        }
    }
}
