using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IDroneItinerarioRepository
    {
        Task<IList<DroneItinerario>> GetAll();
        Task<DroneItinerario> GetById(Guid id);
        Task<DroneItinerario> GetById(int id);
        Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id);

        Task<bool> Insert(DroneItinerario droneItinerario);
        Task<DroneItinerario> Update(DroneItinerario droneItinerario);
        Task<bool> Delete(DroneItinerario droneItinerario);
    }
}
