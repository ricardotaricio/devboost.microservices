using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevBoost.dronedelivery.DTO;
using Microsoft.AspNetCore.Authorization;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.dronedelivery.Domain.Enum;
using DevBoost.dronedelivery.Domain;

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

        // GET: api/Drone
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SituacaoDroneDTO>>> GetDrone()
        {
            await _pedidoService.DespacharPedidos();

            var drones = await _droneService.GetAll();

            IList<SituacaoDroneDTO> situacaoDrones = new List<SituacaoDroneDTO>();

            IList<DroneItinerario> dronesItinerario = _droneItinerarioService.GetAll().Result;

            var pedidosEmTransito = await _pedidoService.GetPedidosEmTransito();

            foreach (var drone in drones)
            {
                SituacaoDroneDTO situacaoDrone = new SituacaoDroneDTO();
                situacaoDrone.Drone = drone;

                var droneItinerario =  dronesItinerario.SingleOrDefault(x => x.DroneId == drone.Id);

                if (droneItinerario == null)
                    situacaoDrone.StatusDrone = EnumStatusDrone.Disponivel.ToString();
                else
                    situacaoDrone.StatusDrone = droneItinerario.StatusDrone.ToString();

                //situacaoDrone.Pedidos = pedidos.Where(p => p.Drone != null && p.Status != EnumStatusPedido.Entregue && p.Drone.Id == drone.Id).ToList();
                situacaoDrone.Pedidos = pedidosEmTransito.Where(p => p.Drone != null && p.Status != EnumStatusPedido.Entregue && p.Drone.Id == drone.Id).ToList();

                situacaoDrones.Add(situacaoDrone);
            }

            return Ok(situacaoDrones);
        }

        // GET: api/Drone/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDrone(int id)
        {
            var drone = await _droneService.GetById(id);

            if (drone == null)
            {
                return NotFound();
            }

            return Ok(drone);
        }
        /*
        // PUT: api/Drone/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDrone(int id, Drone drone)
        {
            if (id != drone.Id)
            {
                return BadRequest();
            }

            _unitOfWork.Entry(drone).State = EntityState.Modified;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DroneExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */

        // POST: api/Drone
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Drone>> PostDrone(DroneDTO droneDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (droneDTO.Autonomia <= 0 || droneDTO.Capacidade <= 0 || droneDTO.Carga <= 0 || droneDTO.Velocidade <= 0)
                return BadRequest("Valores inválidos para Autonomia, Capacidade, Carga e/ou Velocidade.");

            Drone drone = new Drone()
            {
                Velocidade = droneDTO.Velocidade,
                Autonomia = droneDTO.Autonomia,
                AutonomiaRestante = droneDTO.Autonomia,
                Carga = droneDTO.Carga,
                Capacidade = droneDTO.Capacidade
            };

            bool result = await _droneService.Insert(drone);

            if (result)
            {
                DroneItinerario droneItinerario = new DroneItinerario();
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
