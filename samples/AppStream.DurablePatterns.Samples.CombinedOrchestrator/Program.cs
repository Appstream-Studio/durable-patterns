using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services
                        .AddDurablePatterns(cfg => cfg.AddActivitiesFromAssembly(typeof(GetFooItems).Assembly))
                        .AddSingleton<IFooItemRepository, FooItemRepository>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
