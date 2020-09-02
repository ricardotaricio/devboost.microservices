using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevBoost.DroneDelivery.Application.ViewModels
{
    public class AdicionarClienteViewModel
    {
        [Required(ErrorMessage = "Nome é necessário")]
        [StringLength(255, ErrorMessage = "Deve ter entre 2 e 255 caracteres", MinimumLength = 2)]
        [DataType(DataType.Text)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Latitude é necessária")]
        [Range(0, double.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude é necessária")]
        [Range(0, double.MaxValue, ErrorMessage = "Por favor insira um número válido")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "UserName é necessário")]
        [StringLength(255, ErrorMessage = "Deve ter entre 2 e 255 caracteres", MinimumLength = 2)]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Senha é necessária")]
        [StringLength(255, ErrorMessage = "Deve ter entre 5 e 255 caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
