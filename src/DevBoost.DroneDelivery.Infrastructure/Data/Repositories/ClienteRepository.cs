using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DCDroneDelivery _context;

        public IUnitOfWork UnitOfWork => _context;

        public ClienteRepository(DCDroneDelivery context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> ObterTodos()
        {
            return await _context.Cliente
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente> ObterPorId(Guid id)
        {
            return await _context.Cliente.FindAsync(id);
        }

        public async Task<Cliente> GetById(int id)
        {
            return await _context.Cliente.FindAsync(id);
        }

        public async Task Adicionar(Cliente cliente)
        {
            await Task.Run(() => _context.Cliente.Add(cliente));
        }

        public async Task Atualizar(Cliente cliente)
        {
            await Task.Run(() => _context.Cliente.Update(cliente));
        }

        public async Task<IEnumerable<Cliente>> ObterPor(Expression<Func<Cliente, bool>> predicate)
        {
            
            return await Task.Run(() => _context.Cliente.Where(predicate).AsNoTracking().ToListAsync());
        }

        public Task AdicionarRange(IEnumerable<Cliente> entity)
        {
            throw new NotImplementedException();
        }

        public Task AtualizarRange(IEnumerable<Cliente> entities)
        {
            throw new NotImplementedException();
        }

        public Task Excluir(Cliente entity)
        {
            throw new NotImplementedException();
        }

        public Task<Cliente> ObterPorId(int id)
        {
            throw new NotImplementedException();
        }
        public async Task DisposeAsync()
        {
            if (_context != null)
                await _context.DisposeAsync();
        }


    }
}
