using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    [ExcludeFromCodeCoverage]

    public class AdicionarPedidoViewModel
    {
        [Required(ErrorMessage = "Peso é necessário")]
        [Range(1, 12, ErrorMessage = "Pedido fora do peso aceito.")]
        public int Peso { get; set; }
    }
}
