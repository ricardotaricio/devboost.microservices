using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Pagamento.Application.DTOs
{
    public class PagamentoMockApiResponseDTO
    {
        public DateTime Data { get; set; }
        public bool Autorizado { get; set; }
        public string CodigoAutorizacao { get; set; }
    }
}
