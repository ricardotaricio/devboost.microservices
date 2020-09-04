using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]

    public class Cliente
    {
        public Cliente()
        {

        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
