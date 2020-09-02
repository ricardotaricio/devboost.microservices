using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    public abstract class Entity
    { 
        public Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

    }
}
