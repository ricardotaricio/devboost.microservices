using DevBoost.dronedelivery.Domain;
using DevBoost.dronedelivery.Domain.Enum;
using DevBoost.DroneDelivery.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repositoryPedido;
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;
        private readonly IDroneRepository _droneRepository;

        public PedidoService(IPedidoRepository repositoryPedido,
            IDroneItinerarioRepository droneItinerarioRepository,
            IDroneRepository droneRepository)
        {
            _repositoryPedido = repositoryPedido;
            _droneItinerarioRepository = droneItinerarioRepository;
            _droneRepository = droneRepository;
        }

        public async Task<bool> Delete(Pedido pedido)
        {
            return await _repositoryPedido.Delete(pedido);
        }

        public async Task<IList<Pedido>> GetAll()
        {
            return await _repositoryPedido.GetAll();
        }

        public async Task<Pedido> GetById(Guid id)
        {
            return await _repositoryPedido.GetById(id);
        }

        public async Task<Pedido> GetById(int id)
        {
            return await _repositoryPedido.GetById(id);
        }

        public async Task<bool> Insert(Pedido pedido)
        {
            var dronesSitema = await _droneRepository.GetAll();

            if (!dronesSitema.Any(d => d.Capacidade >= pedido.Peso))
                return await Task.Run(() => false);    
            
            //Deve colocar a regra de criação de pedido

            return await _repositoryPedido.Insert(pedido);
        }

        public async Task<Pedido> Update(Pedido pedido)
        {
            return await _repositoryPedido.Update(pedido);
        }
    }
}
