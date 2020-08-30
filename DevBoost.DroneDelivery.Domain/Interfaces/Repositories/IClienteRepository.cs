using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IClienteRepository
    {
        Task<IList<Cliente>> GetAll();
        Task<Cliente> GetById(Guid id);
        Task<bool> Insert(Cliente cliente);
        Task<Cliente> Update(Cliente cliente);
        Task<bool> Delete(Cliente cliente);
    }
}
