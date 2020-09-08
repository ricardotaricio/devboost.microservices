using AutoMapper;
using DevBoost.DroneDelivery.Pagamento.Application.Commands;
using DevBoost.DroneDelivery.Pagamento.Application.DTOs;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;

namespace DevBoost.DroneDelivery.Infrastructure.AutoMapper
{
    public class DtoToCommandMappingProfile : Profile
    {
        public DtoToCommandMappingProfile()
        {
            CreateMap<PagamentoResponseDTO, AtualizarSituacaoPagamentoCartaoCommand>()
                .ConstructUsing(p => new AtualizarSituacaoPagamentoCartaoCommand(p.PagamentoId, p.Autorizado ? SituacaoPagamento.Autorizado : SituacaoPagamento.Negado));
                

        }
    }
}
