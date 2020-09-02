using AutoBogus;
using DevBoost.DroneDelivery.API.Controllers;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevBoost.DroneDelivery.Test.API
{
    public class PedidoControllerTest
    {
        [Fact(DisplayName = "AdicionarPedido")]
        [Trait("PedidoControllerTest", "Controller Tests")]
        public async void AdicionarPedido_test()
        {
            // Given
            var mocker = new AutoMocker();
            var faker = AutoFaker.Create();
            var adicionarPedidoViewModel = faker.Generate<AdicionarPedidoViewModel>();
            var usuario = faker.Generate<User>();

            var identity = new ClaimsIdentity(new Claim[]
            {  new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
               new Claim(ClaimTypes.Role, usuario.Role.ToString())
            });
            var claims = new ClaimsPrincipal(identity);

            var pedidoControllerMock = mocker.CreateInstance<PedidoController>();
            pedidoControllerMock.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(identity) }  };  

            var cliente = usuario.Cliente;
            var pedido = faker.Generate<Pedido>();
            pedido.InformarCliente(cliente);
            var responsePedidoTask = Task.Factory.StartNew(() => pedido);
            var responsUserTask = Task.Run(() => usuario);
            var responeInserirPedidoTask = Task.Factory.StartNew(() => true);
            var pedidoService = mocker.GetMock<IPedidoService>();
            var userService = mocker.GetMock<IUserService>();
            userService.Setup(u => u.GetByUserName(It.IsAny<string>())).Returns(responsUserTask).Verifiable();
            pedidoService.Setup(p => p.IsPedidoValido(It.IsAny<Pedido>())).Returns(string.Empty).Verifiable();
            pedidoService.Setup(p => p.Insert(It.IsAny<Pedido>())).Returns(responeInserirPedidoTask).Verifiable();
            
            
            var expectResponse = pedidoControllerMock.Ok(pedido);

            //When
            var result = await pedidoControllerMock.AdicionarPedido(adicionarPedidoViewModel);
           

            //Then
            var comparison = new CompareLogic();
            pedidoService.Verify(mock => mock.Insert(It.IsAny<Pedido>()), Times.Once());

            Assert.True(comparison.Compare(expectResponse, result).AreEqual);
        }
    }
}
