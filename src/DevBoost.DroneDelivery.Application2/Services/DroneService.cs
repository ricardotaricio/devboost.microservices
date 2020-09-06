using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
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

        public async Task<IEnumerable<Drone>> GetAll()
        {
            return await _droneRepository.ObterTodos();
        }

        public async Task<Drone> GetById(int id)
        {
            return await _droneRepository.ObterPorId(id);
        }

        public async Task<bool> Insert(Drone drone)
        {
             await _droneRepository.Adicionar(drone);
            return await _droneRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Update(Drone drone)
        {
             await _droneRepository.Atualizar(drone);
            return await _droneRepository.UnitOfWork.Commit();
        }
    }
}
