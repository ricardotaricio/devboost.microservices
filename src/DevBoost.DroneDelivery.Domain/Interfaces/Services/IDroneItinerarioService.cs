using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneItinerarioService
    {
        Task<IList<DroneItinerario>> GetAll();
        Task<DroneItinerario> GetById(int id);
        Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id);

        Task<bool> Insert(DroneItinerario droneItinerario);
        Task<DroneItinerario> Update(DroneItinerario droneItinerario);
    }
}
