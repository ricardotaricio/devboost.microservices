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
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.dronedelivery.DTO;

namespace DevBoost.dronedelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IPedidoService _pedidoService;
        private readonly IUserService _userService;

        public PedidoController(IPedidoService pedidoService, IUserService userService)
        {
            _pedidoService = pedidoService;
            _userService = userService;
        }

        // GET: api/Pedido
        [HttpGet, Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedido()
        {
            await _pedidoService.DespacharPedidos();

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

            return Ok(pedido);
        }

        // POST: api/Pedido
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost, Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<Pedido>> PostPedido(PedidoDTO pedidoDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string username = User.Identities.FirstOrDefault().Name;

            User user = await _userService.GetByUserName(username);

            Cliente cliente = user.Cliente;

            if (user.Cliente == null)
                return BadRequest("Usuário não é um Cliente");

            Pedido pedido = new Pedido();
            pedido.InformarPeso(pedidoDTO.Peso);
            pedido.InformarCliente(cliente);
            pedido.InformarHoraPedido(DateTime.Now);
            pedido.InformarStatus(EnumStatusPedido.AguardandoEntregador);

            string motivoRejeicaoPedido = string.Empty;
            if (!_pedidoService.IsPedidoValido(pedido, out motivoRejeicaoPedido))
                return BadRequest("Pedido rejeitado: " + motivoRejeicaoPedido);

            await _pedidoService.Insert(pedido);
                        
            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }
    }
}
