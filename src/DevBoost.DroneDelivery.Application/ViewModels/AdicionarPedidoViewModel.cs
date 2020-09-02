using System.ComponentModel.DataAnnotations;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    public class AdicionarPedidoViewModel
    {
        [Required(ErrorMessage = "Peso é necessário")]
        [Range(1, 12, ErrorMessage = "Pedido fora do peso aceito.")]
        public int Peso { get; set; }
    }
}
