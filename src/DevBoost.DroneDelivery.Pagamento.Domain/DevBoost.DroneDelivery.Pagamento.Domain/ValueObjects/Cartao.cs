using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects
{
    public struct Cartao
    {
        public string Bandeira { get; set; }
        public string Numero { get; set; }
        public short MesVencimento { get; set; }
        public short AnoVencimento { get; set; }
    }
}
