using AutoBogus;
using DevBoost.DroneDelivery.Application.Services;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
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
    public class UserServiceTest
    {
        [Fact(DisplayName = "GetByUserName")]
        [Trait("UserServiceTest", "Service Tests")]
        public async void GetByUserName_test()
        {
            // Given
            var mocker = new AutoMocker();
            var userServiceMock = mocker.CreateInstance<UserService>();

            var faker = AutoFaker.Create();

            var user = faker.Generate<User>();

            var responseUserTask = Task.Factory.StartNew(() => user);

            var expectResponse = user;

            var userRepository = mocker.GetMock<IUserRepository>();

            userRepository.Setup(r => r.GetByUserName(It.IsAny<string>())).Returns(responseUserTask).Verifiable();

            //When
            var result = await userServiceMock.GetByUserName(It.IsAny<string>());

            //Then
            userRepository.Verify(mock => mock.GetByUserName(It.IsAny<string>()), Times.Once());

            CompareLogic comparer = new CompareLogic();
            Assert.True(comparer.Compare(expectResponse, result).AreEqual);
        }

        [Fact(DisplayName = "Authenticate")]
        [Trait("UserServiceTest", "Service Tests")]
        public async void GetAll_test()
        {
            // Given
            var mocker = new AutoMocker();
            var userServiceMock = mocker.CreateInstance<UserService>();

            var faker = AutoFaker.Create();

            var user = faker.Generate<User>();

            var responseUserTask = Task.Factory.StartNew(() => user);

            var expectResponse = user;

            var userRepository = mocker.GetMock<IUserRepository>();

            userRepository.Setup(r => r.GetByUserNameEPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(responseUserTask).Verifiable();

            //When
            var result = await userServiceMock.GetByUserName(It.IsAny<string>());

            //Then
            userRepository.Verify(mock => mock.GetByUserName(It.IsAny<string>()), Times.Once());

            CompareLogic comparer = new CompareLogic();
            Assert.True(comparer.Compare(expectResponse, result).AreEqual);
        }
    }
}
