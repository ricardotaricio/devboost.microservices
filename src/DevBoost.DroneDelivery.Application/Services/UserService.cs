using DevBoost.DroneDelivery.Application.Resources;
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
            password = GetHash(password);

            return await _repositoryUser.ObterPorUserNameEPassword(username, password);

        }

        public async Task<User> GetByUserName(string username)
        {
            return await _repositoryUser.ObterPorUserName(username);
        }

        public async Task Insert(User user)
        {
            user.Password = GetHash(user.Password);

            await _repositoryUser.Atualizar(user);
        }

        private static string GetHash(string input)
        {
            string key = SecretToken.Key;

            input += key;

            var sBuilder = new StringBuilder();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                
            }

            return sBuilder.ToString();
        }
    }
}
