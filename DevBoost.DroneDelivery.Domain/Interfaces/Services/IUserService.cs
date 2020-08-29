using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);

        List<User> GetAll();
    }
}
