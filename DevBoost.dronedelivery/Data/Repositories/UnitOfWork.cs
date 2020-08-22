using DevBoost.dronedelivery.Data.Contexts;
using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PedidoContext _context;

        public UnitOfWork(PedidoContext context)
        {
            _context = context;

            Pedidos = new PedidoRepository(_context);
            Drones = new DroneRepository(_context);
            DroneItinerario = new DroneItinerarioRepository(_context);
        }

        public IPedidoRepository Pedidos { get; private set; }
        public IDroneRepository Drones { get; set; }
        public IDroneItinerarioRepository DroneItinerario { get; set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
