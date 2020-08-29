using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Service
{
    public interface IDeliveryService
    {
        bool IsPedidoValido(Pedido pedido, out string mensagemRejeicaoPedido);
        void DespacharPedidos();
        int CalcularTempoTotalEntregaEmMinutos(IList<Pedido> pedidos, Drone drone);
    }
}
