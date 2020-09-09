using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Application.Commands;
using AutoMapper;

namespace DevBoost.DroneDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        
        private readonly IPedidoService _pedidoService;
        private readonly IUserService _userService;
        private IMediatrHandler _bus;
        private IMapper _mapper;

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
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.Select(x => x.Errors));

            var pedido = await _pedidoService.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return Ok(pedido);
        }

        
        [HttpPost, Authorize(Roles = "ADMIN,USER")]
        public async Task<IActionResult> AdicionarPedido(AdicionarPedidoViewModel pedidoViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.Select(x => x.Errors));


            var username = User.Identities.FirstOrDefault().Name;
            var user = await _userService.GetByUserName(username);
            var cliente = user.Cliente;

            if (user.Cliente == null)
                return BadRequest("Usuário não é um Cliente");

            var pedido = new Pedido(peso: pedidoViewModel.Peso, DateTime.Now, EnumStatusPedido.AguardandoPagamento, valor: pedidoViewModel.Valor);
            pedido.InformarCliente(cliente);

            var motivoRejeicaoPedido = _pedidoService.IsPedidoValido(pedido);
            if (!String.IsNullOrEmpty(_pedidoService.IsPedidoValido(pedido)))
                return BadRequest($"Pedido rejeitado: {motivoRejeicaoPedido}");

            await _pedidoService.Insert(pedido);

            return Ok(pedido);
        }

    }
}
