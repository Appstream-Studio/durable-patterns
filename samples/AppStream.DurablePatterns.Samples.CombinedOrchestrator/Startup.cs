using AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns;
using AppStream.DurablePatterns;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddLogging(cfg =>
                {
                    cfg.AddConsole();
                    cfg.SetMinimumLevel(LogLevel.Trace);
                })
                .AddDurablePatterns()
                .AddDurablePatternsActivitiesFromAssembly(typeof(GetFooItemsActivity).Assembly)
                .AddSingleton<IFooItemRepository, FooItemRepository>();
        }
    }
}
