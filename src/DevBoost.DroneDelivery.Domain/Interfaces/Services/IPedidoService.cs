using DevBoost.dronedelivery.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<IList<Pedido>> GetAll();
        Task<Pedido> GetById(Guid id);
        Task<bool> Insert(Pedido pedido);
        Task<Pedido> Update(Pedido pedido);
        Task<bool> Delete(Pedido pedido);
        bool IsPedidoValido(Pedido pedido, out string mensagemRejeicaoPedido);
        Task DespacharPedidos();
        Task<IList<Pedido>> GetPedidosEmTransito();
    }
}
