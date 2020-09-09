using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class DroneCommandHandler : IRequestHandler<AdicionarDroneCommand, bool>
    {
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;
        private readonly IDroneRepository _droneRepository;


        public DroneCommandHandler(IDroneItinerarioRepository droneItinerarioRepository, IDroneRepository droneRepository)
        {
            _droneItinerarioRepository = droneItinerarioRepository;
            _droneRepository = droneRepository;
        }

        public async Task<bool> Handle(AdicionarDroneCommand request, CancellationToken cancellationToken)
        {
            if (!request.EhValido()) return false;

            var drone = new Drone();
            drone.Capacidade = request.Capacidade;
            drone.Velocidade = request.Velocidade;
            drone.Autonomia = request.Autonomia;
            drone.Carga = request.Carga;
            drone.AutonomiaRestante = request.AutonomiaRestante;

            await _droneRepository.Adicionar(drone);
            var resultDrone = await _droneRepository.UnitOfWork.Commit();

            if (resultDrone)
            {
                var droneItinerario = new DroneItinerario
                {
                    DataHora = request.DataHora,
                    Drone = drone,
                    DroneId = drone.Id,
                    StatusDrone = request.StatusDrone
                };

                await _droneItinerarioRepository.Adicionar(droneItinerario);
                return await _droneItinerarioRepository.UnitOfWork.Commit();
            }
            else 
            { 
                return false;
            }

            
        }
    }
}
