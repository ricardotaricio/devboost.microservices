using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Models
{
    public class Pedido
    {
        public Pedido()
        {

        }

        public Guid Id { get; set; }
        public int Peso { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DataHora { get; set; }
        public Drone Drone { get; set; }
        //public int DroneId { get; set; }
        //public DateTime PrevisaoEntrega { get; set; }
        public EnumStatusPedido Status { get; set; }
    }
}
