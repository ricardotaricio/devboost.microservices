using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevBoost.DroneDelivery.Test.Config
{
    public class DroneDeliveryAppFactory<TStartaup> : WebApplicationFactory<TStartaup> where TStartaup :class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TStartaup>();
            builder.UseEnvironment("Testing");
            base.ConfigureWebHost(builder); 
        }
    }
}
