using AutoMapper;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;

namespace DevBoost.DroneDelivery.Infrastructure.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ClienteViewModel, Cliente>()
                .ConstructUsing(c => new Cliente(c.Nome, c.Latitude, c.Longitude)).ReverseMap();

            CreateMap<DroneViewModel, Drone>()
                .ConstructUsing(d => new Drone(d.Capacidade,d.Velocidade,d.Autonomia,d.AutonomiaRestante)).ReverseMap();
        }

    }
}
