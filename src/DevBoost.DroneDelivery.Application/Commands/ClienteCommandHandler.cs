using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class ClienteCommandHandler : IRequestHandler<AdicionarDroneCommand, bool>
    {
        

        public async Task<bool> Handle(AdicionarDroneCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return false;

            return true;
        }
    }
}
