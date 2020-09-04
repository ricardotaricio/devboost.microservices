using System;
using System.Diagnostics.CodeAnalysis;

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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
