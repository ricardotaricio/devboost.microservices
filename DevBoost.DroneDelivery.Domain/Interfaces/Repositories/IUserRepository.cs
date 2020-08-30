using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUserNameEPassword(string username, string password);
        Task<User> GetByUserName(string username);
        Task<bool> Insert(User user);
    }
}
