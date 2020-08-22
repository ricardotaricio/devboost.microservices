using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IPedidoRepository Pedidos { get; }
        public IDroneRepository Drones { get; }
        public IDroneItinerarioRepository DroneItinerario { get; }

        void Save();
    }
}
