using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
