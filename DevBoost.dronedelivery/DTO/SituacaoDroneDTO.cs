using DevBoost.dronedelivery.Domain;
using System.Collections.Generic;

namespace DevBoost.dronedelivery.DTO
{
    public class SituacaoDroneDTO
    {
        public Drone Drone { get; set; }
        public string StatusDrone { get; set; }
        public IList<Pedido> Pedidos { get; set; }
    }
}
