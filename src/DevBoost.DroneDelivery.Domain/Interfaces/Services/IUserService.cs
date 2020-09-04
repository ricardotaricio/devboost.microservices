using DevBoost.DroneDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> GetByUserName(string username);
        Task Insert(User user);
    }
}
