using System;
using System.Linq;
using System.Threading.Tasks;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DevBoost.DroneDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IUserService _userService;

        public ClienteController(IClienteService clienteService, IUserService userService)
        {
            _clienteService = clienteService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _clienteService.GetAll());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var cliente = await _clienteService.GetById(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AdicionarClienteViewModel  adicionarClienteView)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.Select(x => x.Errors));

            User userExistente = await _userService.GetByUserName(adicionarClienteView.UserName);
            if (userExistente != null)
                return BadRequest("Nome de usuário inválido");

            var cliente = new Cliente() { Nome = adicionarClienteView.Nome, Latitude = adicionarClienteView.Latitude, Longitude = adicionarClienteView.Longitude };
            await _clienteService.Insert(cliente);

            User user = new User(Guid.Empty, adicionarClienteView.UserName, adicionarClienteView.Senha, "USER", cliente);
            await _userService.Insert(user);

            return CreatedAtAction("Get", new { id = cliente.Id }, cliente);
        }
    }
}
