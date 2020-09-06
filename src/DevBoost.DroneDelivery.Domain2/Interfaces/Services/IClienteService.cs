using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IClienteService
    {
        
        Task<IEnumerable<Cliente>> GetAll();
        Task<Cliente> GetById(Guid id);
        Task<bool> Insert(Cliente cliente);
    }
}
