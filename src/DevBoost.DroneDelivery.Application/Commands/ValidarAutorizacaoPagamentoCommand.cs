using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Domain.Entities;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class ValidarAutorizacaoPagamentoCommand : Command
    {

        public ValidarAutorizacaoPagamentoCommand(Pedido pedido, EnumStatusPedido statusPedido)
        {
            Pedido = pedido;
            StatusPedido = statusPedido;
        }

        public Pedido Pedido { get; private set; }

        public EnumStatusPedido StatusPedido { get; private set; }

    }
}
