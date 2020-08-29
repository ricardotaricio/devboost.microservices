using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Device.Location;
using DevBoost.dronedelivery.Domain;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;

namespace DevBoost.dronedelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/Pedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {
            atualizarStatusDrones();
            return Ok(await _pedidoService.GetAll());
        }

        // GET: api/Pedido/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(Guid id)
        {
            var pedido = await _pedidoService.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        // PUT: api/Pedido/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(Guid id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest();
            }

            await _pedidoService.Update(pedido);
                        
            //_unitOfWork.Pedidos.Update(pedido);

            //try
            //{
            //    await Task.Run(
            //    () =>
            //    {
            //        _unitOfWork.Save();
            //    });
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!PedidoExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }

        // POST: api/Pedido
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            // existe drone com capacidade maior que o peso do pedido (limite maximo 12kg)
                      

            //var dronesDisponiveisIds = _unitOfWork.DroneItinerario.GetAll().Where(d => d.StatusDrone == EnumStatusDrone.Disponivel).Select(s => s.DroneId).ToList();

            //if (!dronesDisponiveisIds.Any())
            //    return BadRequest("Rejeitado: Não há entregadores disponíveis.");

            //Drone drone = _unitOfWork.Drones.GetAll().Where(d => dronesDisponiveisIds.Contains(d.Id) && d.Capacidade >= pedido.Peso).FirstOrDefault();

            //if (drone == null)
            //    return BadRequest("Rejeitado: Pedido acima do peso máximo aceito.");
            
            //// calcular distancia do trajeto
            //// calcular tempo total (ida e volta) do trajeto (limite maximo 35m)
            //// existe um drone que atende essas condicoes

            //double distancia = CalcularDistanciaEmKilometros((double)pedido.Latitude, (double)pedido.Longitude);
            //distancia = distancia * 2;

            //// tempo = distancia / velocidade
            //// 80km / 40km/h = 2h
            //double tempoTrajetoCompleto = distancia / drone.Velocidade;
            //tempoTrajetoCompleto = tempoTrajetoCompleto * 60;

            //if (tempoTrajetoCompleto > drone.Autonomia)
            //    return BadRequest("Rejeitado: Fora da área de entrega.");

            ////todo: Tempo médio de carga de bateria de TODOS os Drone: 1 hora

            //pedido.DataHora = DateTime.Now;
            //pedido.PrevisaoEntrega = DateTime.Now.AddMinutes(Convert.ToInt32(tempoTrajetoCompleto / 2));
            //pedido.Drone = drone;
            //pedido.Status = EnumStatusPedido.EmTransito;

            //DroneItinerario droneItinerario = _unitOfWork.DroneItinerario.GetAll().Where(i => i.DroneId == drone.Id).FirstOrDefault();

            //if (droneItinerario == null)
            //{
            //    droneItinerario = new DroneItinerario();

            //    droneItinerario.DataHora = DateTime.Now;
            //    droneItinerario.Drone = drone;
            //    droneItinerario.StatusDrone = EnumStatusDrone.EmTransito;

            //    _unitOfWork.DroneItinerario.Insert(droneItinerario);
            //}
            //else
            //{
            //    droneItinerario.DataHora = DateTime.Now;
            //    droneItinerario.Drone = drone;
            //    droneItinerario.StatusDrone = EnumStatusDrone.EmTransito;

            //    _unitOfWork.DroneItinerario.Update(droneItinerario);
            //}

            //drone.AutonomiaRestante = drone.AutonomiaRestante - Convert.ToInt32(tempoTrajetoCompleto);

            //_unitOfWork.Pedidos.Insert(pedido);
            //_unitOfWork.Drones.Update(drone);
            
            //await Task.Run(
            //() =>
            //{
            //    _unitOfWork.Save();
            //});

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);


        }

        // DELETE: api/Pedido/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> DeletePedido(Guid id)
        {

            var pedido = await _pedidoService.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            var result = await _pedidoService.Delete(pedido);

            if (!result)
                return BadRequest();

            return Ok(pedido);
        }

        private bool PedidoExists(Guid id)
        {
            //var pedido = _unitOfWork.Pedidos.GetById(id);

            //if (pedido == null)
            //    return false;

            return true;
        }
               
        private double CalcularDistanciaEmKilometros(double latitudeDestino, double longitudeDestino)
        {
            var origemCoord = new GeoCoordinate(-23.5880684, -46.6564195); //local delivery
            var destinoCoord = new GeoCoordinate(latitudeDestino, longitudeDestino);

            var distance = origemCoord.GetDistanceTo(destinoCoord);

            distance = distance / 1000;

            return distance;
        }

        private void atualizarStatusDrones()
        {
            // lista itinerario nao disponíveis

            //var droneItinerarios = _unitOfWork.DroneItinerario.GetAll().Where(d => d.StatusDrone != EnumStatusDrone.Disponivel).ToList();

            //foreach (var droneItinerario in droneItinerarios)
            //{
            //    droneItinerario.Drone = _unitOfWork.Drones.GetById(droneItinerario.DroneId);

            //    if (droneItinerario.StatusDrone == EnumStatusDrone.Carregando)
            //    {
            //        if (DateTime.Now.Subtract(droneItinerario.DataHora).Minutes >= 60)
            //        {
            //            droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;
            //            droneItinerario.Drone.AutonomiaRestante = droneItinerario.Drone.Autonomia;
            //            droneItinerario.DataHora = DateTime.Now;
            //        }
            //    }
            //    else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
            //    {
            //        var pedido = _unitOfWork.Pedidos.GetAll().Where(p => p.DroneId == droneItinerario.DroneId && p.Status == EnumStatusPedido.EmTransito).FirstOrDefault();

            //        int tempoEntrega = pedido.PrevisaoEntrega.Subtract(pedido.DataHora).Minutes;

            //        if (pedido.PrevisaoEntrega.AddMinutes(tempoEntrega) <= DateTime.Now)
            //        {
            //            if (droneItinerario.Drone.AutonomiaRestante <= 5)
            //                droneItinerario.StatusDrone = EnumStatusDrone.Carregando;
            //            else
            //                droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;

            //            droneItinerario.DataHora = DateTime.Now;
            //            pedido.Status = EnumStatusPedido.Entregue;

            //            _unitOfWork.Pedidos.Update(pedido);
            //        }
            //    }

            //    _unitOfWork.DroneItinerario.Update(droneItinerario);
            //}

            //_unitOfWork.Save();
        }
    }
}
