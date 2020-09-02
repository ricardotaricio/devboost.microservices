using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.ValueObjects
{
    public struct Localizacao
    {
        public Localizacao(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

    }
}
