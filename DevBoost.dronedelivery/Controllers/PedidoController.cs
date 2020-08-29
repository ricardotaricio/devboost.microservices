using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Device.Location;
using DevBoost.dronedelivery.Domain;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using DevBoost.dronedelivery.Domain.Enum;

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
        [HttpGet, Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {            
            return Ok(await _pedidoService.GetAll());
        }

        // GET: api/Pedido/5
        [HttpGet("{id}"), Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<Pedido>> GetPedido(Guid id)
        {
            var pedido = await _pedidoService.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        //// PUT: api/Pedido/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
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
        [HttpPost, Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string motivoRejeicaoPedido = string.Empty;
            if (!_pedidoService.IsPedidoValido(pedido, out motivoRejeicaoPedido))
                return BadRequest("Pedido rejeitado: " + motivoRejeicaoPedido);

            pedido.InformarHoraPedido(DateTime.Now);
            pedido.InformarStatus(EnumStatusPedido.AguardandoEntregador);

            await _pedidoService.Insert(pedido);
                        
            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedido/5
        [HttpDelete("{id}"), Authorize(Roles = "ADMIN,USER")]
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
    }
}
