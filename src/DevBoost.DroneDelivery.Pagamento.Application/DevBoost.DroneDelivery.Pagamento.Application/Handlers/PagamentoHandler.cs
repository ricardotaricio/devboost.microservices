using AutoMapper;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Pagamento.Application.Commands;
using DevBoost.DroneDelivery.Pagamento.Application.DTOs;
using DevBoost.DroneDelivery.Pagamento.Application.Events;
using DevBoost.DroneDelivery.Pagamento.Application.Queries;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.Application.Handlers
{
    public class PagamentoHandler : INotificationHandler<ProcessarPagamentoCartaoEvent>
    {
        private readonly IPagamentoQueries _pagamentoQueries;
        private readonly IMediatrHandler _bus;
        private readonly IMapper _mapper;

        public PagamentoHandler(IPagamentoQueries pagamentoQueries, IMediatrHandler bus, IMapper mapper)
        {
            _pagamentoQueries = pagamentoQueries;
            _bus = bus;
            _mapper = mapper;
        }

        public async Task Handle(ProcessarPagamentoCartaoEvent message, CancellationToken cancellationToken)
        {
            var pagamentoCartao = await _pagamentoQueries.ObterPorId(message.EntityId);

            if (pagamentoCartao == null)
                return;

            var body = _mapper.Map<PagamentoRequestDTO>(pagamentoCartao);
           
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync("https://5f542997e5de110016d51dec.mockapi.io/v1/pagamento", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var objResponse = JsonConvert.DeserializeObject<PagamentoResponseDTO>(await response.Content.ReadAsStringAsync());
                    objResponse.PagamentoId = pagamentoCartao.Id;
                    await _bus.EnviarComando(_mapper.Map<AtualizarSituacaoPagamentoCartaoCommand>(objResponse));
                }
            }
        }
    }
}
