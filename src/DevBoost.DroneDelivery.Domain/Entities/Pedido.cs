using DevBoost.Dronedelivery.Domain.Enumerators;
using System;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    public class Pedido : Entity
    {
        public Pedido(int peso, DateTime dataHora,EnumStatusPedido status)
        {
            Peso = peso;
            DataHora = dataHora;
            Status = status;
        }

        public int Peso { get; set; }
        public DateTime DataHora { get; private set; }
        public Drone Drone { get; set; }
        public int DroneId { get; set; }
        public EnumStatusPedido Status { get; private set; }
        public Cliente Cliente { get; private set; }

        public void InformarHoraPedido(DateTime horaPedido)
        {
            this.DataHora = horaPedido;
        }

        public void AtualizarStatus(EnumStatusPedido statusPedido)
        {
            Status = statusPedido;
        }

        public void InformarDrone(Drone drone)
        {
            Drone = drone;
        }

        public void InformarStatus(EnumStatusPedido status)
        {
            Status = status;
        }

        public void InformarCliente(Cliente cliente)
        {
            Cliente = cliente;
        }

        public void InformarPeso(int peso)
        {
            Peso = peso;
        }
    }
}
