using DevBoost.dronedelivery.Domain;
using DevBoost.DroneDelivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Repository.Context
{
    public class DCDroneDelivery : DbContext
    {
        public DCDroneDelivery(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Drone> Drone { get; set; }
        public DbSet<DroneItinerario> DroneItinerario { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
    }
}
