using DevBoost.dronedelivery.Data.Contexts;
using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public class DroneRepository : EFRepository<Drone>, IDroneRepository
    {
        private readonly PedidoContext _context;

        public DroneRepository(PedidoContext context) : base(context)
        {
            this._context = context;
        }
    }
}
