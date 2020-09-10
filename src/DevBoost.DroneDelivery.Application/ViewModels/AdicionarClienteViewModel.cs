using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AdicionarClienteViewModel
    {
        [Required(ErrorMessage = "Nome é necessário")]
        [StringLength(255, ErrorMessage = "Deve ter entre 2 e 255 caracteres", MinimumLength = 2)]
        [DataType(DataType.Text)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Latitude é necessária")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude é necessária")]
        [Range(double.MinValue,double.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "UserName é necessário")]
        [StringLength(255, ErrorMessage = "Deve ter entre 2 e 255 caracteres", MinimumLength = 2)]
        [DataType(DataType.Text)]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Senha é necessária")]
        [StringLength(255, ErrorMessage = "Deve ter entre 5 e 255 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
