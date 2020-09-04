using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
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

        

        public async Task<IEnumerable<Cliente>> GetAll()
        {
            return await _repositoryCliente.ObterTodos();
        }

        public async Task<Cliente> GetById(Guid id)
        {
            return await _repositoryCliente.ObterPorId(id);
        }

        public async Task Insert(Cliente cliente)
        {

            await _repositoryCliente.Adicionar(cliente);
        }

        public async Task<Cliente> Update(Cliente cliente)
        {
            return await _repositoryCliente.Atualizar(cliente);
        }
    }
}
