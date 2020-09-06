using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Contexts
{
    [ExcludeFromCodeCoverage]

    public class DCDroneDelivery : BaseDbContext
    {
        public DCDroneDelivery(DbContextOptions options, IMediatrHandler bus) : base(options, bus)
        {
        }

        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Drone> Drone { get; set; }
        public DbSet<DroneItinerario> DroneItinerario { get; set; }
        public DbSet<Usuario> User { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
    }
}
