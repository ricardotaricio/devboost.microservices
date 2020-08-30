
using DevBoost.dronedelivery.Domain.Enum;
using DevBoost.DroneDelivery.Domain.Entities;
using System;

namespace DevBoost.dronedelivery.Domain
{
    public class Pedido
    {
        public Pedido(){}

        public Guid Id { get; set; }
        public int Peso { get; set; }
        //public decimal Latitude { get; set; }
        //public decimal Longitude { get; set; }
        public DateTime DataHora { get; set; }
        public Drone Drone { get; set; }
        public int? DroneId { get; set; }
        //public DateTime PrevisaoEntrega { get; set; }
        public EnumStatusPedido Status { get; set; }
        public Cliente Cliente { get; private set; }

        public void InformarHoraPedido(DateTime horaPedido)
        {
            this.DataHora = horaPedido;
        }

        //public void InformarPrevisaoEntrega(DateTime dataPrevista)
        //{
        //    this.PrevisaoEntrega = dataPrevista;
        //}

        public void InformarDrone(Drone drone)
        {
            this.Drone = drone;
        }

        public void InformarStatus(EnumStatusPedido status)
        {
            this.Status = status;
        }

        public void InformarCliente(Cliente cliente)
        {
            this.Cliente = cliente;
        }

        public void InformarPeso(int peso)
        {
            this.Peso = peso;
        }
    }
}
