using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DCDroneDelivery _context;

        public ClienteRepository(DCDroneDelivery context)
        {
            this._context = context;
        }

        public async Task<bool> Delete(Cliente cliente)
        {
            _context.Cliente.Remove(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IList<Cliente>> GetAll()
        {
            return await _context.Cliente
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente> GetById(Guid id)
        {
            return await _context.Cliente.FindAsync(id);
        }

        public async Task<Cliente> GetById(int id)
        {
            return await _context.Cliente.FindAsync(id);
        }

        public async Task<bool> Insert(Cliente cliente)
        {
            _context.Cliente.Add(cliente);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Cliente> Update(Cliente cliente)
        {
            _context.Cliente.Update(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }
    }
}
