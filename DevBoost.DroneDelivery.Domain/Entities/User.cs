using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    public class User
    {
        public User()
        {

        }

        public User(Guid id, string username, string password, string role, Cliente cliente)
        {
            Id = id;
            UserName = username;
            Password = password;
            Role = role;
            Cliente = cliente;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public Cliente Cliente { get; set; }
    }
}
