using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class Entity
    { 
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

    }
}
