using DevBoost.DroneDelivery.API.DTO;
using DevBoost.DroneDelivery.Application.Services;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public IActionResult Authenticate([FromBody] LoginViewModel  loginViewModel)
        {


            var user = _userService.Authenticate(loginViewModel.Nome, loginViewModel.Senha).Result;

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            return Ok(TokenService.GenerateToken(user));
        }
    }
}
