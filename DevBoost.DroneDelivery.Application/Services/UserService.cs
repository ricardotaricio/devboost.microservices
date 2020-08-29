using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class UserService : IUserService
    {
        public User Authenticate(string username, string password)
        {
            var user = GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }

        public List<User> GetAll()
        {
            return new List<User>()
            {
                new User(Guid.NewGuid(), "Mirosmar", "123", "ADMIN"),
                new User(Guid.NewGuid(), "Givanildo", "123", "USER"),
                new User(Guid.NewGuid(), "DevBoot", "123", "USER")
            };
        }
    }
}
