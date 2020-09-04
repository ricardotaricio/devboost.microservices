using DevBoost.DroneDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> ObterPorUserNameEPassword(string username, string password);
        Task<User> ObterPorUserName(string username);
  
    }
}
