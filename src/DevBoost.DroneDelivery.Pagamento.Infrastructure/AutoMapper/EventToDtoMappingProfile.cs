using AutoMapper;
using DevBoost.DroneDelivery.Pagamento.Application.Events;
using DevBoost.DroneDelivery.Pagamento.Application.ViewModels;

namespace DevBoost.DroneDelivery.Pagamento.Infrastructure.AutoMapper
{
    public class EventToDtoMappingProfile : Profile
    {
        public EventToDtoMappingProfile()
        {
            CreateMap<PagamentoCartaoAdicionadoEvent, AtualizarSituacaoPedidoViewModel>()
                .ForMember(d => d.PedidoId, o => o.MapFrom(o => o.EntityId))
                .ForMember(d => d.PagamentoId, o => o.MapFrom(o => o.PagamentoId))
                .ForMember(d => d.SituacaoPagamento, o => o.MapFrom(o => o.SituacaoPagamento));
        }
    }
}
