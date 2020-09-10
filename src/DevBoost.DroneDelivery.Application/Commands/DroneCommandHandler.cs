using AutoMapper;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Application.Events;
using DevBoost.DroneDelivery.Application.Queries;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.JSInterop.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class DroneCommandHandler : IRequestHandler<AdicionarDroneCommand, bool>, IRequestHandler<AtualizarAutonomiaDroneCommand, bool>
    {
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;
        private readonly IDroneRepository _droneRepository;
        private readonly IMediatrHandler _mediatr;
        private readonly IMapper _mapper;
        private readonly IDroneQueries _droneQueries;
        public DroneCommandHandler(IDroneQueries droneQueries, IMapper mapper, IMediatrHandler mediatr, IDroneItinerarioRepository droneItinerarioRepository, IDroneRepository droneRepository)
        {
            _droneItinerarioRepository = droneItinerarioRepository;
            _droneRepository = droneRepository;
            _mediatr = mediatr;
            _mapper = mapper;
            _droneQueries = droneQueries;
        }

        public async Task<bool> Handle(AdicionarDroneCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;


            var drone = _mapper.Map<Drone>(message);
            await _droneRepository.Adicionar(drone);

            drone.AdicionarEvento(_mapper.Map<DroneAdicionadoEvent>(drone));
            return await _droneRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarAutonomiaDroneCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var drone = _mapper.Map<Drone>(_droneQueries.ObterPorId(message.DroneId));
            drone.InformarAutonomiaRestante(message.AutonomiaRestante);
            drone.AdicionarEvento(new AutonomiaAtualizadaDroneEvent(drone.Id, drone.AutonomiaRestante));
            return await _droneRepository.UnitOfWork.Commit();

        }

        private bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                _mediatr.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));
            }

            return false;
        }
    }
}
