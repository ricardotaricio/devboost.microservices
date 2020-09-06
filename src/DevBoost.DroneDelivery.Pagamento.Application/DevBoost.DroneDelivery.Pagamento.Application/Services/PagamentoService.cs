using DevBoost.DroneDelivery.Pagamento.Application.Interfaces.Services;
using DevBoost.DroneDelivery.Pagamento.Application.ViewModels;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using DevBoost.DroneDelivery.Pagamento.Domain.Interfaces.Repositories;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentoService(IPagamentoRepository pagamentoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
        }

        public Task<bool> Processar(AdicionarPagamentoCartaoViewModel adicionarPagamentoCartaoViewModel)
        {
            //if (!adicionarPagamentoCartaoViewModel.EhValido())
            //    return Task.Run(() => false);

            PagamentoCartao pagamentoCartao = new PagamentoCartao()
            {
                PedidoId = adicionarPagamentoCartaoViewModel.PedidoId,
                Valor = adicionarPagamentoCartaoViewModel.Valor,
                Cartao = adicionarPagamentoCartaoViewModel.Cartao,
                Situacao = SituacaoPagamento.Aguardando
            };

            _pagamentoRepository.Adicionar(pagamentoCartao);

            return _pagamentoRepository.UnitOfWork.Commit();
        }
    }
}
