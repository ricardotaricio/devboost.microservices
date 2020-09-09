using DevBoost.DroneDelivery.Core.Domain.Messages;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class AdicionarUsuarioCommand : Command
    {
        public AdicionarUsuarioCommand(string nome, double latitude, double longitude, string userName, string password, string role)
        {
            Nome = nome;
            Latitude = latitude;
            Longitude = longitude;
            UserName = userName;
            Password = password;
            Role = role;

        }

        public string Nome { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
