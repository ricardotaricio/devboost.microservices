using DevBoost.Dronedelivery.Domain.Enumerators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]
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
