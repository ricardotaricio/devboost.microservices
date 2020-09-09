using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    public class AdicionarPagamentoCartaoViewModel
    {
        public Guid PedidoId { get; set; }
        public double Valor { get; set; }
        public string BandeiraCartao { get; set; }
        public string NumeroCartao { get; set; }
        public short MesVencimentoCartao { get; set; }
        public short AnoVencimentoCartao { get; set; }
    }
}
