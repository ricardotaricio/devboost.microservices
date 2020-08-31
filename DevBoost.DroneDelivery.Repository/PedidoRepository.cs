using DevBoost.dronedelivery.Domain;
using DevBoost.dronedelivery.Domain.Enum;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Repository
{
    public class PedidoRepository : Repository, IPedidoRepository
    {
        private readonly DCDroneDelivery _context;

        public PedidoRepository(DCDroneDelivery context)
        {
            this._context = context;
        }

        public async Task<bool> Delete(Pedido pedido)
        {
            _context.Pedido.Remove(pedido);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IList<Pedido>> GetAll()
        {
            return await _context.Pedido
                .Include(p => p.Drone)
                .AsNoTracking()
                .Include(p => p.Cliente)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pedido> GetById(Guid id)
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Pedido> GetById(int id)
        {
            return await _context.Pedido.FindAsync(id);
        }

        public async Task<bool> Insert(Pedido pedido)
        {
            _context.Entry(pedido.Cliente).State = EntityState.Unchanged;
            
            //_context.Entry(pedido.Drone).State = EntityState.Unchanged;

            _context.Pedido.Add(pedido);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Pedido> Update(Pedido pedido)
        {
            //bool tracking = _context.ChangeTracker.Entries<Cliente>().Any(x => x.Entity.Id == pedido.Cliente.Id);

            //if (!tracking)
            //    _context.Entry(pedido.Cliente).State = EntityState.Unchanged;

            DetachLocal<Pedido>(_context, p => p.Id == pedido.Id);
            DetachLocal<Cliente>(_context, c => c.Id == pedido.Cliente.Id);
            DetachLocal<Drone>(_context, d => d.Id == pedido.Drone.Id);

            _context.Pedido.Update(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }
                        
        public async Task<IList<Pedido>> GetPedidosEmAberto()
        {
            return await _context.Pedido
                .Include(p => p.Cliente).AsNoTracking()
                .Where(p => p.Status == EnumStatusPedido.AguardandoEntregador).ToListAsync();
        }

        public async Task<IList<Pedido>> GetPedidosEmTransito()
        {
            return await _context.Pedido
                .Include(p => p.Cliente).AsNoTracking()
                .Include(p => p.Drone).AsNoTracking()
                .Where(p => p.Status == EnumStatusPedido.EmTransito)
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
