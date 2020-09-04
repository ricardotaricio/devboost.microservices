using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class DroneItinerarioService : IDroneItinerarioService
    {
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;

        public DroneItinerarioService(IDroneItinerarioRepository droneItinerarioRepository)
        {
            _droneItinerarioRepository = droneItinerarioRepository;
        }

        

        public async Task<IEnumerable<DroneItinerario>> GetAll()
        {
            return await _droneItinerarioRepository.ObterTodos();
        }

        public async Task<DroneItinerario> GetById(Guid id)
        {
            return await _droneItinerarioRepository.ObterPorId(id);
        }

        public async Task<DroneItinerario> GetById(int id)
        {
            return await _droneItinerarioRepository.ObterPorId(id);
        }

        public async Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id)
        {
            return await _droneItinerarioRepository.ObterDroneItinerarioPorIdDrone(id);
        }

        public async Task Insert(DroneItinerario droneItinerario)
        {
            await _droneItinerarioRepository.Adicionar(droneItinerario);
        }

        public async Task<DroneItinerario> Update(DroneItinerario droneItinerario)
        {
            return await _droneItinerarioRepository.Atualizar(droneItinerario);
        }
    }
}
