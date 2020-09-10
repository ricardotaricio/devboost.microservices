using DevBoost.DroneDelivery.Core.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<Pedido>> GetAll();
        Task<Pedido> GetById(Guid id);
        Task<bool> Insert(Pedido pedido);
        Task<bool> Update(Pedido pedido);
       
        string IsPedidoValido(Pedido pedido);
        Task DespacharPedidos();
        Task<IEnumerable<Pedido>> GetPedidosEmTransito();
    }
}
