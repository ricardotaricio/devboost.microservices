using DevBoost.DroneDelivery.Core.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]

    public class Drone : Entity
    {
        public Drone(){}

        public Drone(int capacidade, int velocidade, int autonomia, int autonomiaRestante, int carga)
        {
            Capacidade = capacidade;
            Velocidade = velocidade;
            Autonomia = autonomia;
            AutonomiaRestante = autonomiaRestante;
            Carga = carga;
        }

        public int Capacidade { get; private set; }
        public int Velocidade { get; private set; }
        public int Autonomia { get; private set; }
        public int AutonomiaRestante { get; private set; }
        public int Carga { get; private set; }

        public void InformarAutonomiaRestante(int autonomia)
        {
            this.AutonomiaRestante = autonomia;
        }
    }
}
