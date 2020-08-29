using DevBoost.dronedelivery.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Repositories
{
    public interface IDroneRepository
    {
        Task<IList<Drone>> GetAll();
        Task<Drone> GetById(Guid id);
        Task<Drone> GetById(int id);
        Task<bool> Insert(Drone drone);
        Task<Drone> Update(Drone drone);
        Task<bool> Delete(Drone drone);
    }
}
