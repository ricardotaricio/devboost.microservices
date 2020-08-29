using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevBoost.dronedelivery.DTO;
using Microsoft.AspNetCore.Authorization;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.dronedelivery.Domain.Enum;
using DevBoost.dronedelivery.Domain;

namespace DevBoost.dronedelivery.Controllers
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
        [HttpGet, Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<IEnumerable<SituacaoDroneDTO>>> GetDrone()
        {

            var drones = await _droneService.GetAll();

            IList<SituacaoDroneDTO> situacaoDrones = new List<SituacaoDroneDTO>();

            foreach (var drone in drones)
            {
                SituacaoDroneDTO situacaoDrone = new SituacaoDroneDTO();
                situacaoDrone.Drone = drone;

                var droneItinerario =  _droneItinerarioService.GetAll().Result.SingleOrDefault(x => x.DroneId == drone.Id);

                if (droneItinerario == null)
                    situacaoDrone.StatusDrone = EnumStatusDrone.Disponivel.ToString();
                else
                    situacaoDrone.StatusDrone = droneItinerario.StatusDrone.ToString();

                var pedidos = await _pedidoService.GetAll();                 

                situacaoDrone.Pedidos = pedidos.Where(p => p.Drone != null && p.Status != EnumStatusPedido.Entregue && p.Drone.Id == drone.Id).ToList(); ;

                situacaoDrones.Add(situacaoDrone);
            }

            return Ok(situacaoDrones);
        }

        // GET: api/Drone/5
        [HttpGet("{id}"), Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<Drone>> GetDrone(int id)
        {
            var drone = await _droneService.GetById(id);

            if (drone == null)
            {
                return NotFound();
            }

            return drone;
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

        // POST: api/Drone
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Drone>> PostDrone(Drone drone)
        {
            _unitOfWork.Drone.Add(drone);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction("GetDrone", new { id = drone.Id }, drone);
        }

        // DELETE: api/Drone/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Drone>> DeleteDrone(int id)
        {
            var drone = await _unitOfWork.Drone.FindAsync(id);
            if (drone == null)
            {
                return NotFound();
            }

            _unitOfWork.Drone.Remove(drone);
            await _unitOfWork.SaveChangesAsync();

            return drone;
        }
        */

        private bool DroneExists(int id)
        {            
            return (_droneService.GetById(id).Result != null);
        }
    }
}
