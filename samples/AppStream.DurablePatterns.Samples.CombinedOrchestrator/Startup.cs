using AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns;
using AppStream.DurablePatterns;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities;
using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddDurablePatterns()
                .AddDurablePatternsActivitiesFromAssembly(typeof(GetFooItemsActivity).Assembly)
                .AddSingleton<IFooItemRepository, FooItemRepository>();
        }
    }
}
