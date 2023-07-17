using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json", optional: false))
                .ConfigureServices(services =>
                {
                    services
                        .AddDurablePatterns(cfg => cfg.AddActivitiesFromAssembly(typeof(GetFooItems).Assembly))
                        .AddSingleton<IFooItemRepository, FooItemRepository>();
                })
                .ConfigureLogging((hostContext, logging) => logging.AddConfiguration(hostContext.Configuration.GetSection("Logging")))
                .Build();

            await host.RunAsync();
        }
    }
}
