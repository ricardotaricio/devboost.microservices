﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Usuario
    {
        public Usuario()
        {

        }

        public Usuario(Guid id, string username, string password, string role, Cliente cliente)
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
