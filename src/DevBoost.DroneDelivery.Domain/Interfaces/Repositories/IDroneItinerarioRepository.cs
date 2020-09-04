using DevBoost.DroneDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IDroneItinerarioRepository : IRepository<DroneItinerario>
    {
        Task<DroneItinerario> ObterDroneItinerarioPorIdDrone(int id);

    }
}
