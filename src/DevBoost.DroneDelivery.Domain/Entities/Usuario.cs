using DevBoost.DroneDelivery.Core.Domain.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace DevBoost.DroneDelivery.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Usuario : Entity
    {
        public Usuario()
        {

        }

        public Usuario(string username, string password, string role,Guid ClienteId)
        {
            
            UserName = username;
            Password = password;
            Role = role;
        }

        
        public string UserName { get; set; }

        private string password;

        public string Password
        {
            get { return getHash(password); }
            set { password = getHash(value); }
        }

        
        public string Role { get; set; }
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }


        private static string getHash(string input)
        {
            string key = "dronedelivery";

            input += key;

            var sBuilder = new StringBuilder();

            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }


            return sBuilder.ToString();
        }
    }
}
