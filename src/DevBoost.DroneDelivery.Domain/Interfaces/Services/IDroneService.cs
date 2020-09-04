using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneService
    {
        Task<IEnumerable<Drone>> GetAll();
        Task<Drone> GetById(Guid id);
        Task<Drone> GetById(int id);
        Task Insert(Drone drone);
        Task<Drone> Update(Drone drone);
        
    }
}
