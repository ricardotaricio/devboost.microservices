using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repositoryUser;

        public UserService(IUserRepository repositoryUser)
        {
            _repositoryUser = repositoryUser;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            return await _repositoryUser.GetByUserNameEPassword(username, password);

            //var user = GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
            //return user;
        }

        public async Task<User> GetByUserName(string username)
        {
            return await _repositoryUser.GetByUserName(username);
        }

        public async Task<bool> Insert(User user)
        {
            return await _repositoryUser.Insert(user);
        }


        //public List<User> GetAll()
        //{
        //    return new List<User>()
        //    {
        //        new User(Guid.NewGuid(), "Mirosmar", "123", "ADMIN"),
        //        new User(Guid.NewGuid(), "Givanildo", "123", "USER"),
        //        new User(Guid.NewGuid(), "DevBoot", "123", "USER")
        //    };
        //}
    }
}
