using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Controllers
{
    public class SituacaoDroneDTO
    {
        public Drone Drone { get; set; }
        public string StatusDrone { get; set; }
        public IList<Pedido> Pedidos { get; set; }
    }
}
