using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;

namespace DevBoost.DroneDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        
        private readonly IPedidoService _pedidoService;
        private readonly IUserService _userService;

        public PedidoController(IPedidoService pedidoService, IUserService userService)
        {
            _pedidoService = pedidoService;
            _userService = userService;
        }

        
        [HttpGet, Authorize(Roles = "ADMIN,USER")]
        public async Task<IActionResult> GetPedido()
        {
            await _pedidoService.DespacharPedidos();

            return Ok(await _pedidoService.GetAll());
        }

       
        [HttpGet("{id}"), Authorize(Roles = "ADMIN,USER")]
        public async Task<IActionResult> GetPedido(Guid id)
        {
            var pedido = await _pedidoService.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return Ok(pedido);
        }

        
        [HttpPost, Authorize(Roles = "ADMIN,USER")]
        public async Task<IActionResult> PostPedido(AdicionarPedidoViewModel pedidoViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string username = User.Identities.FirstOrDefault().Name;

            var user = await _userService.GetByUserName(username);

            var cliente = user.Cliente;

            if (user.Cliente == null)
                return BadRequest("Usuário não é um Cliente");

            var pedido = new Pedido();
            pedido.InformarPeso(pedidoViewModel.Peso);
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
