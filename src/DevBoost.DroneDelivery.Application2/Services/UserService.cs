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

        public async Task<Usuario> Authenticate(string username, string password)
        {
            password = getHash(password);

            return await _repositoryUser.ObterCredenciais(username, password);

        }

        public async Task<Usuario> GetByUserName(string username)
        {
            return await _repositoryUser.ObterPorNome(username);
        }

        public async Task<bool> Insert(Usuario user)
        {
            user.Password = getHash(user.Password);

            await _repositoryUser.Adicionar(user);
            return await _repositoryUser.UnitOfWork.Commit();
        }

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
