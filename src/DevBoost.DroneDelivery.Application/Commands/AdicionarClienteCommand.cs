using DevBoost.DroneDelivery.Application.Validations;
using DevBoost.DroneDelivery.Core.Domain.Messages;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class AdicionarClienteCommand : Command
    {
        public AdicionarClienteCommand(string nome, double latitude, double longitude)
        {
            Nome = nome;
            Latitude = latitude;
            Longitude = longitude;
            
        }

        public string Nome { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private  set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarClienteValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
