using DevBoost.DroneDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneItinerarioService
    {
        Task<IEnumerable<DroneItinerario>> GetAll();
        Task<DroneItinerario> GetById(int id);
        Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id);

        Task<bool> Insert(DroneItinerario droneItinerario);
        Task<bool> Update(DroneItinerario droneItinerario);
    }
}
