using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;


namespace DevBoost.DroneDelivery.API.Controllers
{
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public OAuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Authenticate([FromBody] LoginViewModel  loginViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.Select(x => x.Errors));

            var user = await _userService.Authenticate(loginViewModel.Nome, loginViewModel.Senha);

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            return Ok(TokenGenerator.GenerateToken(user));
        }
    }
}
