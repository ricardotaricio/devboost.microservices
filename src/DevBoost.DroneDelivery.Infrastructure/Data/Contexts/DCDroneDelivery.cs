using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Contexts
{
    public class DCDroneDelivery : DbContext, IUnitOfWork
    {
        public DCDroneDelivery(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Drone> Drone { get; set; }
        public DbSet<DroneItinerario> DroneItinerario { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Cliente> Cliente { get; set; }

        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
