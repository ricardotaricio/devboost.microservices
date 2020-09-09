﻿
using DevBoost.DroneDelivery.Core.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Pagamento.Domain.Entites
{
    [ExcludeFromCodeCoverage]
    public class Cartao : Entity
    {


        public Cartao() : base()
        {

        }

        public string Bandeira { get; set; }
        public string Numero { get; set; }
        public int MesVencimento { get; set; }
        public int AnoVencimento { get; set; }

        public PagamentoCartao  PagamentoCartao { get; set; }
    }
}
