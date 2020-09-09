using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Pagamento.Application.Events
{
    public class ProcessarPagamentoCartaoEvent : Event
    {

       


        public ProcessarPagamentoCartaoEvent(Guid entityId): base(entityId)
        {

        }
    }
}
