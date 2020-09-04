using DevBoost.DroneDelivery.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using Xunit;

namespace DevBoost.DroneDelivery.Test.Config
{

    [CollectionDefinition(nameof(IntegrationApiTestFixtureCollection))]

    public class IntegrationApiTestFixtureCollection : ICollectionFixture<IntegrationTestFixture<StartupApiTests>> { }
    public class IntegrationTestFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly DroneDeliveryAppFactory<TStartup> factory;
        public HttpClient client;

        public IntegrationTestFixture()
        {

            var clientOptions = new WebApplicationFactoryClientOptions
            {

            };
            factory = new DroneDeliveryAppFactory<TStartup>();
            client = factory.CreateClient();
        }
        public void Dispose()
        {
            client?.Dispose();
            factory?.Dispose();
        }
    }
}
