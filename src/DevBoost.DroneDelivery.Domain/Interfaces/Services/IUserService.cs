using DevBoost.DroneDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Usuario> Authenticate(string username, string password);
        Task<Usuario> GetByUserName(string username);
        
    }
}
