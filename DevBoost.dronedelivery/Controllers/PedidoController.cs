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
using System.Device.Location;
using DevBoost.dronedelivery.Service;

namespace DevBoost.dronedelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeliveryService _deliveryService;

        public PedidoController(IUnitOfWork unitOfWork, IDeliveryService deliveryService)
        {
            _unitOfWork = unitOfWork;
            _deliveryService = deliveryService;
        }

        // GET: api/Pedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {
            _deliveryService.DespacharPedidos();

            return _unitOfWork.Pedidos.GetAll().ToList();
        }

        // GET: api/Pedido/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(Guid id)
        {
            var pedido = _unitOfWork.Pedidos.GetById(id);

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

            _unitOfWork.Pedidos.Update(pedido);

            try
            {
                await Task.Run(
                () =>
                {
                    _unitOfWork.Save();
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
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

        // POST: api/Pedido
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //todo: retornar mensagem
            double tempoTrajetoCompleto = 0;
            if (!_deliveryService.IsPedidoValido(pedido, out tempoTrajetoCompleto))
                return BadRequest("Pedido rejeitado.");

            pedido.DataHora = DateTime.Now;
            //todo: remover campo previsao entrega do banco de dados
            //pedido.PrevisaoEntrega = pedido.DataHora.AddMinutes(Convert.ToInt32(tempoTrajetoCompleto / 2));
            pedido.Status = EnumStatusPedido.AguardandoEntregador;

            _unitOfWork.Pedidos.Insert(pedido);

            await Task.Run(
            () =>
            {
                _unitOfWork.Save();
            });

            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedido/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> DeletePedido(Guid id)
        {
            var pedido = _unitOfWork.Pedidos.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            _unitOfWork.Pedidos.Delete(pedido);
            
            await Task.Run(
            () =>
            {
                _unitOfWork.Save();
            });

            return pedido;
        }

        private bool PedidoExists(Guid id)
        {
            var pedido = _unitOfWork.Pedidos.GetById(id);

            if (pedido == null)
                return false;

            return true;
        }
    }
}
