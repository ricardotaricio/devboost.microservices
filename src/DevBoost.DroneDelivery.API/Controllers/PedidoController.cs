using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Application.Events;
using DevBoost.DroneDelivery.Application.Queries;

namespace DevBoost.DroneDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoQueries _pedidoQueries;
        private readonly IPedidoService _pedidoService;
        private readonly IUserService _userService;
        private readonly IMediatrHandler _mediator;
 
        public PedidoController(IPedidoQueries pedidoQueries, IPedidoService pedidoService, IUserService userService, IMediatrHandler mediatr)
        {
            _pedidoService = pedidoService;
            _userService = userService;
            _mediator = mediatr;
            _pedidoQueries = pedidoQueries;
        }


        //[HttpGet, Authorize(Roles = "ADMIN,USER")]
        [HttpGet]
        public async Task<IActionResult> GetPedido()
        {
            return Ok(await _pedidoQueries.ObterTodos());
        }


        //[HttpGet("{id}"), Authorize(Roles = "ADMIN,USER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(Guid id)
        {

            var pedido = await _pedidoQueries.ObterPorId(id);

            if (pedido == null) return NotFound();
           
            return Ok(pedido);
        }

        // TODO: Refatorar
        //[HttpPost, Authorize(Roles = "ADMIN,USER")]
        [HttpPost]
        public async Task<IActionResult> AdicionarPedido(AdicionarPedidoViewModel pedidoViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.Select(x => x.Errors));

            var username = User.Identities.FirstOrDefault().Name;
            var user = await _userService.GetByUserName(username);
            var cliente = user.Cliente;

            if (user.Cliente == null)
                return BadRequest("Usuário não é um Cliente");

            var pedido = new Pedido(peso: pedidoViewModel.Peso, DateTime.Now, EnumStatusPedido.AguardandoPagamento, pedidoViewModel.Valor);
            pedido.InformarCliente(cliente);

            //TODO: Verificar codigo comentado!!!

            //var motivoRejeicaoPedido = _pedidoService.IsPedidoValido(pedido);
            //if (!String.IsNullOrEmpty(_pedidoService.IsPedidoValido(pedido)))
            //    return BadRequest($"Pedido rejeitado: {motivoRejeicaoPedido}");

            //bool pedidoInserido = await _pedidoService.Insert(pedido);

            //if (!pedidoInserido)
            //    return BadRequest();

            using (HttpClient client = new HttpClient())
            {
                var body = new AdicionarPagamentoCartaoViewModel()
                {
                    PedidoId = pedido.Id,
                    Valor = pedidoViewModel.Valor,
                    NumeroCartao = pedidoViewModel.NumeroCartao,
                    BandeiraCartao = pedidoViewModel.Bandeira,
                    MesVencimentoCartao = pedidoViewModel.MesVencimento,
                    AnoVencimentoCartao = pedidoViewModel.AnoVencimento
                };

                var response = await client.PostAsync("https://localhost/api/pagamento", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    return BadRequest();
            }

            return Ok(pedido);
        }


        [HttpPatch]
        public async Task<IActionResult> AtualizarSituacaoPedido(AtualizarSituacaoPedidoViewModel viewModel)
        {
            await _mediator.PublicarEvento(new PagementoPedidoProcessadoEvent(viewModel.PedidoId, viewModel.SituacaoPagamento));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> DespacharPedidos()
        {
            await _pedidoService.DespacharPedidos();
            return Ok();
        }
    }
}
