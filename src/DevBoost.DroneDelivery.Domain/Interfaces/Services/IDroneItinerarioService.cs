using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneItinerarioService
    {
        Task<IEnumerable<DroneItinerario>> GetAll();
        Task<DroneItinerario> GetById(Guid id);
        Task<DroneItinerario> GetById(int id);
        Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id);

        Task Insert(DroneItinerario droneItinerario);
        Task<DroneItinerario> Update(DroneItinerario droneItinerario);
        
    }
}
