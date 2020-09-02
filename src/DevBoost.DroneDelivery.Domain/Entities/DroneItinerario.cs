using DevBoost.dronedelivery.Domain.Enumerators;
using System;

namespace DevBoost.dronedelivery.Domain
{
    public class DroneItinerario
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public Drone Drone { get; set; }
        public int DroneId { get; set; }
        public EnumStatusDrone StatusDrone { get; set; }

        public void InformarStatusDrone(EnumStatusDrone statusDrone)
        {
            this.StatusDrone = statusDrone;
        }

        public void InformarDrone(Drone drone)
        {
            this.Drone = drone;
        }

        public void InformarDataHora(DateTime data)
        {
            this.DataHora = data;
        }
    }
}
