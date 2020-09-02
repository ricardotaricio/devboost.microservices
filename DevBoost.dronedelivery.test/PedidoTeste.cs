using AutoBogus;
using DevBoost.dronedelivery.Controllers;
using DevBoost.dronedelivery.Domain;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevBoost.dronedelivery.test
{
    public class PedidoTeste
    {
        [Fact]
        public async void Validar_Pedido_Com_Cliente_Dentro_Da_Area_De_Entrega_Com_Sucesso()
        {
            // Given
            var mocker = new AutoMocker();
            var baseControllerMock = mocker.CreateInstance<DroneController>();

            var faker = AutoFaker.Create();

            var response = faker.Generate<Drone>();

            var responseTask = Task.Factory.StartNew(() => response);

            var expectResponse = baseControllerMock.Ok(response);

            var droneService = mocker.GetMock<IDroneService>();

            droneService.Setup(r => r.GetById(It.IsAny<Int32>())).Returns(responseTask).Verifiable();

            //When
            var result = await baseControllerMock.GetDrone(It.IsAny<Int32>());

            //Then
            var comparison = new CompareLogic();
            droneService.Verify(mock => mock.GetById(It.IsAny<Int32>()), Times.Once());
            Assert.True(comparison.Compare(expectResponse, result).AreEqual);
        }
    }
}
