using AutoMapper;
using DevBoost.DroneDelivery.Pagamento.Application.Commands;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;

namespace DevBoost.DroneDelivery.Infrastructure.AutoMapper
{
    public class CommandToDomainMappingProfile : Profile
    {
        public CommandToDomainMappingProfile()
        {
            CreateMap<AdicionarPagamentoCartaoCommand, PagamentoCartao>()
                .ForMember(d => d.Cartao,
                o => o.MapFrom(o => new Cartao()
                {
                    Numero = o.NumeroCartao,
                    MesVencimento = o.MesVencimentoCartao,
                    AnoVencimento = o.AnoVencimentoCartao,
                    Bandeira = o.BandeiraCartao,

                }))
                .ForMember(d => d.Situacao, o => o.MapFrom(o => SituacaoPagamento.Aguardando));
        }

    }
}
