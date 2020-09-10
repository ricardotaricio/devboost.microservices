using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Application.Events
{
    public class DroneAdicionadoEvent : Event
    {
        
        public DroneAdicionadoEvent(Guid entityId) : base(entityId)
        {
            
        }

        public int Capacidade { get;  set; }
        public int Velocidade { get;  set; }
        public int Autonomia { get;  set; }
        public int AutonomiaRestante { get;  set; }
        public int Carga { get;  set; }
    }
}
