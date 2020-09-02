using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repositoryCliente;

        public ClienteService(IClienteRepository repositoryCliente)
        {
            _repositoryCliente = repositoryCliente;
        }

        public async Task<bool> Delete(Cliente cliente)
        {
            return await _repositoryCliente.Delete(cliente);
        }

        public async Task<IList<Cliente>> GetAll()
        {
            return await _repositoryCliente.GetAll();
        }

        public async Task<Cliente> GetById(Guid id)
        {
            return await _repositoryCliente.GetById(id);
        }

        public async Task<bool> Insert(Cliente cliente)
        {

            return await _repositoryCliente.Insert(cliente);
        }

        public async Task<Cliente> Update(Cliente cliente)
        {
            return await _repositoryCliente.Update(cliente);
        }
    }
}
