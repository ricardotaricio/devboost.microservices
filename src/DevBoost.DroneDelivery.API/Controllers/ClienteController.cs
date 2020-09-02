using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBoost.dronedelivery.DTO;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<ClienteController>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> Get()
        {
            return Ok(await _clienteService.GetAll());
        }

        // GET api/<ClienteController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> Get(Guid id)
        {
            var cliente = await _clienteService.GetById(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST api/<ClienteController>
        [HttpPost]
        public async Task<ActionResult<Cliente>> Post([FromBody] ClienteDTO clienteDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User userExistente = await _userService.GetByUserName(clienteDTO.UserName);
            if (userExistente != null)
                return BadRequest("Nome de usuário inválido");

            Cliente cliente = new Cliente() { Nome = clienteDTO.Nome, Latitude = clienteDTO.Latitude, Longitude = clienteDTO.Longitude };
            await _clienteService.Insert(cliente);

            User user = new User(Guid.Empty, clienteDTO.UserName, clienteDTO.Password, "USER", cliente);
            await _userService.Insert(user);

            return CreatedAtAction("Get", new { id = cliente.Id }, cliente);
        }
    }
}
