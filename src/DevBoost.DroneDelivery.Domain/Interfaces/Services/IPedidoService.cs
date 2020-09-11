using DevBoost.DroneDelivery.Core.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
       
        Task DespacharPedidos();
        
    }
}
