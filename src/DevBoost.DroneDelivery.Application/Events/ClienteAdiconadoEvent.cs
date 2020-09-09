using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Application.Events
{
    public class ClienteAdiconadoEvent : Event
    {


        public ClienteAdiconadoEvent(Guid entityId, string nome, string usuario, string senha, double latitude, double longitude) : base(entityId)
        {
            Nome = nome;
            Usuario = usuario;
            Senha = senha;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Nome { get; private set; }
        public string Usuario { get; private set; }
        public string Senha { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
    }
}
