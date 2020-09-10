using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    [ExcludeFromCodeCoverage]

    public class DroneViewModel
    {
        [Required(ErrorMessage = "Capacidade é necessária")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public int Capacidade { get; set; }

        [Required(ErrorMessage = "Velocidade é necessária")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public int Velocidade { get; set; }
        
        [Required(ErrorMessage = "Autonomia é necessária")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public int Autonomia { get; set; }

        [Required(ErrorMessage = "Carga é necessária")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public int Carga { get; set; }


        [Required(ErrorMessage = "Autonomia restante é necessária")]
        [Range(0, int.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public int AutonomiaRestante { get; private set; }
    }
}
