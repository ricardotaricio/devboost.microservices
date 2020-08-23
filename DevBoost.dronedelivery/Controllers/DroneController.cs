using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevBoost.dronedelivery.Data.Contexts;
using DevBoost.dronedelivery.Models;
using DevBoost.dronedelivery.Data.Repositories;

namespace DevBoost.dronedelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DroneController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DroneController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Drone
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SituacaoDroneDTO>>> GetDrone()
        {
            var drones = _unitOfWork.Drones.GetAll();

            IList<SituacaoDroneDTO> situacaoDrones = new List<SituacaoDroneDTO>();

            foreach (var drone in drones)
            {
                SituacaoDroneDTO situacaoDrone = new SituacaoDroneDTO();
                situacaoDrone.Drone = drone;

                //todo: criar itinerario drone na atualização de status se ainda não existir
                DroneItinerario droneItinerario = _unitOfWork.DroneItinerario.GetAll().Where(i => i.DroneId == drone.Id).FirstOrDefault();

                if (droneItinerario == null)
                    situacaoDrone.StatusDrone = EnumStatusDrone.Disponivel.ToString();
                else
                    situacaoDrone.StatusDrone = droneItinerario.StatusDrone.ToString();

                var pedidos = _unitOfWork.Pedidos.GetAll().Where(p => p.Drone != null && p.Status != EnumStatusPedido.Entregue && p.Drone.Id == drone.Id).ToList();

                situacaoDrone.Pedidos = pedidos;

                situacaoDrones.Add(situacaoDrone);
            }

            return Ok(situacaoDrones);
        }

        // GET: api/Drone/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Drone>> GetDrone(int id)
        {
            var drone = _unitOfWork.Drones.GetById(id);

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
            return (_unitOfWork.Drones.GetById(id) != null);
        }
    }
}
