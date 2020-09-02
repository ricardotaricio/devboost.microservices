using System.ComponentModel.DataAnnotations;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    public class LoginViewModel
    {



        [Required(ErrorMessage = "Nome é necessário")]
        [StringLength(255, ErrorMessage = "Deve ter entre 2 e 255 caracteres", MinimumLength = 2)]
        [DataType(DataType.Text)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Senha é necessária")]
        [StringLength(255, ErrorMessage = "Deve ter entre 5 e 255 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

    }
}
