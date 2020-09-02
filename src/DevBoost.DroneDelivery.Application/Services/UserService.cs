using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System.Security.Cryptography;
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
            password = getHash(password);

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
            user.Password = getHash(user.Password);

            return await _repositoryUser.Insert(user);
        }

        private static string getHash(string input)
        {
            string key = "dronedelivery";

            input = input + key;

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
