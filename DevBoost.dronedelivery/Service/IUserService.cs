using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Service
{
    public interface IUserService
    {
        User Authenticate(string username, string password);

        List<User> GetAll();

    }
}
