using AutoBogus;
using DevBoost.DroneDelivery.Application.Services;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevBoost.DroneDelivery.Test.Application
{
    public class PedidoServiceTest
    {
        [Fact(DisplayName = "GetById")]
        [Trait("PedidoServiceTest", "Service Tests")]
        public async void GetById_test()
        {
            // Given
            var mocker = new AutoMocker();
            var pedidoServiceMock = mocker.CreateInstance<PedidoService>();

            var faker = AutoFaker.Create();

            var drone = faker.Generate<Drone>();

            // var pedido = faker.Generate<Pedido>();
            var pedidoFaker = new AutoFaker<Pedido>()
                .RuleFor(u => u.DroneId, f => f.Random.Int(1));

            var pedido = pedidoFaker.Generate();

            var responseDroneTask = Task.Factory.StartNew(() => drone);
            var responsePedidoTask = Task.Factory.StartNew(() => pedido);

            var expectResponse = pedido; // pedidoServiceMock.GetById(responsePedido);

            var droneRepository = mocker.GetMock<IDroneRepository>();
            var pedidoRepository = mocker.GetMock<IPedidoRepository>();

            droneRepository.Setup(r => r.GetById(It.IsAny<Int32>())).Returns(responseDroneTask).Verifiable();
            pedidoRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(responsePedidoTask).Verifiable();

            //When
            var result = await pedidoServiceMock.GetById(It.IsAny<Guid>());

            //Then

            droneRepository.Verify(mock => mock.GetById(It.IsAny<Int32>()), Times.Once());
            pedidoRepository.Verify(mock => mock.GetById(It.IsAny<Guid>()), Times.Once());

            CompareLogic comparer = new CompareLogic();
            Assert.True(comparer.Compare(expectResponse, result).AreEqual);
        }

        [Fact(DisplayName = "GetAll")]
        [Trait("PedidoServiceTest", "Service Tests")]
        public async void GetAll_test()
        {
            // Given
            var mocker = new AutoMocker();
            var pedidoServiceMock = mocker.CreateInstance<PedidoService>();

            var faker = AutoFaker.Create();

            var pedidos = faker.Generate<IList<Pedido>>();
            //var pedidoFaker = new AutoFaker<IList<Pedido>>();

            var responsePedidoTask = Task.Factory.StartNew(() => pedidos);

            var expectResponse = pedidos;

            var pedidoRepository = mocker.GetMock<IPedidoRepository>();

            pedidoRepository.Setup(r => r.GetAll()).Returns(responsePedidoTask).Verifiable();

            //When
            var result = await pedidoServiceMock.GetAll();

            //Then
            pedidoRepository.Verify(mock => mock.GetAll(), Times.Once());

            CompareLogic comparer = new CompareLogic();
            Assert.True(comparer.Compare(expectResponse, result).AreEqual);
        }
    }
}
