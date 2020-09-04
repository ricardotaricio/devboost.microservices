using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class DroneRepository : Repository<Drone>, IDroneRepository
    {
        private readonly DCDroneDelivery _context;

        public DroneRepository(DCDroneDelivery context):base(context)
        {
            _context = context;
        }

    }
}
