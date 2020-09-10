using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Application.Events
{
    public class AutonomiaAtualizadaDroneEvent : Event
    {

        public AutonomiaAtualizadaDroneEvent(Guid entityId , int autonomiaRestante ) :base(entityId)
        {
            AutonomiaRestante = autonomiaRestante;
        }

        public int AutonomiaRestante { get; private set; }
    }
}
