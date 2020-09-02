using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class DroneService : IDroneService
    {
        private readonly IDroneRepository _droneRepository;

        public DroneService(IDroneRepository droneRepository)
        {
            _droneRepository = droneRepository;
        }

        public async Task<bool> Delete(Drone drone)
        {
            return await _droneRepository.Delete(drone);
        }

        public async Task<IList<Drone>> GetAll()
        {
            return await _droneRepository.GetAll();
        }

        public async Task<Drone> GetById(Guid id)
        {
            return await _droneRepository.GetById(id);
        }

        public async Task<Drone> GetById(int id)
        {
            return await _droneRepository.GetById(id);
        }

        public async Task<bool> Insert(Drone drone)
        {
            return await _droneRepository.Insert(drone);
        }

        public async Task<Drone> Update(Drone drone)
        {
            return await _droneRepository.Update(drone);
        }
    }
}
