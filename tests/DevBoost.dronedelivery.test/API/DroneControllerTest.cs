using AutoBogus;
using DevBoost.DroneDelivery.API.Controllers;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.AutoMock;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevBoost.DroneDelivery.Test.API
{
    public class DroneControllerTest
    {
        [Fact(DisplayName = "GetById")]
        [Trait("DroneControllerTest", "Controller Tests")]
        public async void GetById_test()
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
