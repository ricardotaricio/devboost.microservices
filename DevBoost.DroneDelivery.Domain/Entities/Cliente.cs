using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    public class Cliente
    {
        public Cliente()
        {

        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
