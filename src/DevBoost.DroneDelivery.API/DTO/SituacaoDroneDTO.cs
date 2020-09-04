using DevBoost.DroneDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.API.DTO
{
    [ExcludeFromCodeCoverage]

    public class SituacaoDroneDTO
    {
        public Drone Drone { get; set; }
        public string StatusDrone { get; set; }
        public IList<Pedido> Pedidos { get; set; }
    }
}
