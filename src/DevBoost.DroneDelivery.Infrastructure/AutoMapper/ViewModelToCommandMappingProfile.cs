using AutoMapper;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Application.ViewModels;

namespace DevBoost.DroneDelivery.Infrastructure.AutoMapper
{
    public class ViewModelToCommandMappingProfile : Profile
    {

        public ViewModelToCommandMappingProfile()
        {
            CreateMap<AtualizarSituacaoPedidoViewModel, AtualizarSituacaoPedidoCommand>();
                
        }
    }
}
