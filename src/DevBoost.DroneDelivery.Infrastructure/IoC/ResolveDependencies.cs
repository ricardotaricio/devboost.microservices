using AutoMapper;
using DevBoost.DroneDelivery.Application.Bus;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Application.Events;
using DevBoost.DroneDelivery.Application.Queries;
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
            //TODO: Matar esses services!!!
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IUserService, UserService>();
 
            
            services.AddScoped<IDroneItinerarioRepository, DroneItinerarioRepository>();
            services.AddScoped<IDroneRepository, DroneRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            //Queries Cliente
            services.AddScoped<IClienteQueries, ClienteQueries>();

            //Queries Drone
            services.AddScoped<IDroneQueries, DroneQueries>();

            //Event liente
            services.AddScoped<INotificationHandler<ClienteAdiconadoEvent>, ClienteEventHandler>();

            //Event Drone
            services.AddScoped<INotificationHandler<AutonomiaAtualizadaDroneEvent>, DroneEventHandler>();
            services.AddScoped<INotificationHandler<DroneAdicionadoEvent>, DroneEventHandler>();
            services.AddScoped<INotificationHandler<AutonomiaAtualizadaDroneEvent>, DroneEventHandler>();


            //Command Cliente 
            services.AddScoped<IRequestHandler<AdicionarClienteCommand, bool>, ClienteCommandHandler>();


            //Command Drone 
            services.AddScoped<IRequestHandler<AdicionarDroneCommand, bool>, DroneCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarAutonomiaDroneCommand, bool>, DroneCommandHandler>();

            //Command Usuario
            services.AddScoped<IRequestHandler<AdicionarUsuarioCommand, bool>, UsuarioCommandHandler>();

            //Command Pedido
            services.AddScoped<IRequestHandler<AdicionarPedidoCommand, bool>, PedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarSituacaoPedidoCommand, bool>, PedidoCommandHandler>();

            //Command Drone Itinerario 
            services.AddScoped<IRequestHandler<AdicionarDroneItinerarioCommand, bool>, DroneItinerarioCommandHandler>();

            


            TokenGenerator.TokenConfig = configuration.GetSection("Token").Get<Token>();

            var assembly = AppDomain.CurrentDomain.Load("DevBoost.DroneDelivery.Application");
            services.AddMediatR(assembly);
            services.AddTransient<IMediatrHandler, MediatrHandler>();

            
            services.AddAutoMapper(typeof(DtoToCommandMappingProfile), 
                typeof(CommandToDomainMappingProfile), 
                typeof(ViewModelToCommandMappingProfile), 
                typeof(DomainToDtoMappingProfile),
                typeof(ViewModelToDomainMappingProfile));

            

            services.AddDbContext<DCDroneDelivery>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            


            return services;
        }
    }
}
