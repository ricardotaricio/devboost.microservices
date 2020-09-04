using DevBoost.DroneDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Interfaces.Services
{
    public interface IDroneService
    {
        Task<IList<Drone>> GetAll();
        Task<Drone> GetById(int id);
        Task<bool> Insert(Drone drone);
        Task<Drone> Update(Drone drone);
    }
}
