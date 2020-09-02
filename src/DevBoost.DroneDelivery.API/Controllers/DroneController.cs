using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.API.DTO;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.ViewModels;

namespace DevBoost.DroneDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DroneController : ControllerBase
    {
        private readonly IDroneService _droneService;
        private readonly IDroneItinerarioService _droneItinerarioService;
        private readonly IPedidoService _pedidoService;

        public DroneController(IDroneService droneService, IDroneItinerarioService droneItinerarioService, IPedidoService pedidoService)
        {
            _droneService = droneService;
            _droneItinerarioService = droneItinerarioService;
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDrone()
        {
            await _pedidoService.DespacharPedidos();

            var drones = await _droneService.GetAll();

            var situacaoDrones = new List<SituacaoDroneDTO>();

            var dronesItinerario = _droneItinerarioService.GetAll().Result;

            var pedidosEmTransito = await _pedidoService.GetPedidosEmTransito();

            foreach (var drone in drones)
            {
                var situacaoDrone = new SituacaoDroneDTO { Drone = drone };

                var droneItinerario = dronesItinerario.SingleOrDefault(x => x.DroneId == drone.Id);

                if (droneItinerario == null)
                    situacaoDrone.StatusDrone = EnumStatusDrone.Disponivel.ToString();
                else
                    situacaoDrone.StatusDrone = droneItinerario.StatusDrone.ToString();

                situacaoDrone.Pedidos = pedidosEmTransito.Where(p => p.Drone != null && p.Status != EnumStatusPedido.Entregue && p.Drone.Id == drone.Id).ToList();

                situacaoDrones.Add(situacaoDrone);
            }

            return Ok(situacaoDrones);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDrone(int id)
        {
            var drone = await _droneService.GetById(id);

            if (drone == null)
                return NotFound();

            return Ok(drone);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostDrone(AdicionarDroneViewModel droneViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var drone = new Drone()
            {
                Velocidade = droneViewModel.Velocidade,
                Autonomia = droneViewModel.Autonomia,
                AutonomiaRestante = droneViewModel.Autonomia,
                Carga = droneViewModel.Carga,
                Capacidade = droneViewModel.Capacidade
            };

            bool result = await _droneService.Insert(drone);

            if (result)
            {
                var droneItinerario = new DroneItinerario();
                droneItinerario.DataHora = System.DateTime.Now;
                droneItinerario.Drone = drone;
                droneItinerario.DroneId = drone.Id;
                droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;

                await _droneItinerarioService.Insert(droneItinerario);
            }

            return CreatedAtAction("GetDrone", new { id = drone.Id }, drone);
        }
    }
}
