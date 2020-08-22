using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Models
{
    public class DroneItinerario
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public Drone Drone { get; set; }
        public int DroneId { get; set; }
        public EnumStatusDrone StatusDrone { get; set; }
    }
}
