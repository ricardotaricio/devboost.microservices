using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Pagamento.Application.DTOs
{
    [ExcludeFromCodeCoverage]
    public class PagamentoResponseDTO
    {
        public Guid PagamentoId { get; set; }
        public DateTime Data { get; set; }
        public bool Autorizado { get; set; }
        public string CodigoAutorizacao { get; set; }
    }
}
