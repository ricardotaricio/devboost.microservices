using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Pagamento.Application.Commands;
using DevBoost.DroneDelivery.Pagamento.Application.DTOs;
using DevBoost.DroneDelivery.Pagamento.Application.Events;
using DevBoost.DroneDelivery.Pagamento.Application.Queries;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.Application.Handlers
{
    public class PagamentoHandler : INotificationHandler<ProcessarPagamentoCartaoEvent>
    {
        private readonly IPagamentoQueries _pagamentoQueries;
        private readonly IMediatrHandler _bus;

        public PagamentoHandler(IPagamentoQueries pagamentoQueries, IMediatrHandler bus)
        {
            _pagamentoQueries = pagamentoQueries;
            _bus = bus;
        }

        public async Task Handle(ProcessarPagamentoCartaoEvent message, CancellationToken cancellationToken)
        {
            var pagamentoCartao = await _pagamentoQueries.ObterPorId(message.EntityId);

            if (pagamentoCartao == null)
                return;

            PagamentoMockApiDTO pagamentoMockApiDTO = new PagamentoMockApiDTO()
            {
                PagamentoId = pagamentoCartao.Id,
                PedidoId = pagamentoCartao.PedidoId,
                Valor = pagamentoCartao.Valor,
                NumeroCartao = pagamentoCartao.Cartao.Numero,
                Bandeira = pagamentoCartao.Cartao.Bandeira,
                MesVencimento = pagamentoCartao.Cartao.MesVencimento,
                AnoVencimento = pagamentoCartao.Cartao.AnoVencimento
            };

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(
                    "https://5f542997e5de110016d51dec.mockapi.io/v1/pagamento", 
                    new StringContent(JsonConvert.SerializeObject(pagamentoMockApiDTO), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                { 
                    PagamentoMockApiResponseDTO pagamentoMockApiResponseDTO = JsonConvert.DeserializeObject<PagamentoMockApiResponseDTO>(await response.Content.ReadAsStringAsync());

                    var command = new AtualizarSituacaoPagamentoCartaoCommand(
                        pagamentoMockApiDTO.PagamentoId,
                        (pagamentoMockApiResponseDTO.Autorizado ? SituacaoPagamento.Autorizado : SituacaoPagamento.Negado),
                        pagamentoCartao.PedidoId,
                        pagamentoCartao.Valor);

                    await _bus.EnviarComando(command);
                }
            }
        }
    }
}
