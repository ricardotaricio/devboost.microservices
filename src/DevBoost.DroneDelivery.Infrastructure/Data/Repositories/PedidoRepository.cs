using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly DCDroneDelivery _context;

        public PedidoRepository(DCDroneDelivery context)
        {
            this._context = context;
        }

       
        public async Task<IEnumerable<Pedido>> ObterTodos()
        {
            return await _context.Pedido
                .Include(p => p.Drone)
                .AsNoTracking()
                .Include(p => p.Cliente)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pedido> ObterPorId(Guid id)
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Pedido> ObterPorId(int id)
        {
            return await _context.Pedido.FindAsync(id);
        }

        public async Task Adicionar(Pedido pedido)
        {
            _context.Entry(pedido.Cliente).State = EntityState.Unchanged;
           await Task.Run(()=> _context.Pedido.Add(pedido));
        }

        public async Task<Pedido> Atualizar(Pedido pedido)
        {
            var retorno = await Task.FromResult(_context.Pedido.Update(pedido));
            return retorno.Entity;
        } 
                        
        public async Task<IEnumerable<Pedido>> ObterPedidosEmAberto()
        {
            return await _context.Pedido
                .Include(p => p.Cliente).AsNoTracking()
                .Where(p => p.Status == EnumStatusPedido.AguardandoEntregador).ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> ObterPedidosEmTransito()
        {
            return await _context.Pedido
                .Include(p => p.Cliente).AsNoTracking()
                .Include(p => p.Drone).AsNoTracking()
                .Where(p => p.Status == EnumStatusPedido.EmTransito)
                .ToListAsync();
        }

        public void DisposeAsync()
        {
            _context.Dispose();
        }
    }
}
