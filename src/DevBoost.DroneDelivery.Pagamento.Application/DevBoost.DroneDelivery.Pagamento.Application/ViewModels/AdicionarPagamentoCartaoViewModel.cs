using DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects;
using System;

namespace DevBoost.DroneDelivery.Pagamento.Application.ViewModels
{
    public class AdicionarPagamentoCartaoViewModel
    {
        public Guid PedidoId { get; set; }
        public double Valor { get; set; }
        public Cartao Cartao { get; set; }
    }
}
