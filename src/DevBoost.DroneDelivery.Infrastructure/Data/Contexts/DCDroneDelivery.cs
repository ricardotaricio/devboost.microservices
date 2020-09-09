using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Contexts
{
    [ExcludeFromCodeCoverage]

    public class DCDroneDelivery : BaseDbContext
    {

        private IMediatrHandler _Bus;
        public DCDroneDelivery()
        {

        }
        public DCDroneDelivery(DbContextOptions<DCDroneDelivery> options, IMediatrHandler bus) : base(options, bus)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Server=NTB040\\SQLEXPRESS;Database=DroneDelivery;Trusted_Connection=true;")
                    .UseLazyLoadingProxies(false);
            }

            
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Drone> Drone { get; set; }
        public DbSet<DroneItinerario> DroneItinerario { get; set; }
        public DbSet<Usuario> User { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
    }
}
