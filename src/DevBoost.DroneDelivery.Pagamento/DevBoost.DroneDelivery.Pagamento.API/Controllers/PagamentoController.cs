using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Pagamento.Application.Commands;
using DevBoost.DroneDelivery.Pagamento.Application.Interfaces.Services;
using DevBoost.DroneDelivery.Pagamento.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentoController : ControllerBase
    {
        private IMediatrHandler _bus;

        public PagamentoController(IMediatrHandler bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> PagamentoCartaoPost(AdicionarPagamentoCartaoViewModel adicionarPagamentoCartaoViewModel)
        {
            var command = new AdicionarPagamentoCartaoCommand(
                adicionarPagamentoCartaoViewModel.PedidoId, 
                adicionarPagamentoCartaoViewModel.Valor, 
                adicionarPagamentoCartaoViewModel.Cartao);

            // TODO: Verificar como retornar motivo da rejeição do pagamento a partir do service pra não precisar validar aqui, apenas no service.
            var enviado = await _bus.EnviarComando(command);

            if (!enviado) return BadRequest();

            return Ok();
        }
    }
}
