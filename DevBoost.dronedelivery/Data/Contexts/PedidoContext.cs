using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBoost.dronedelivery.Models;

namespace DevBoost.dronedelivery.Data.Contexts
{
    public class PedidoContext : DbContext
    {
        public PedidoContext()
        {

        }

        public PedidoContext(DbContextOptions<PedidoContext> options):base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DroneDelivery;Trusted_Connection=true;");
            }
        }

        public DbSet<DevBoost.dronedelivery.Models.Pedido> Pedido { get; set; }
        public DbSet<DevBoost.dronedelivery.Models.Drone> Drone { get; set; }
        public DbSet<DevBoost.dronedelivery.Models.DroneItinerario> DroneItinerario { get; set; }

    }
}
