using DevBoost.DroneDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneService
    {
        Task<IEnumerable<Drone>> GetAll();
        Task<Drone> GetById(int id);
        Task<bool> Insert(Drone drone);
        Task<bool> Update(Drone drone);
    }
}
