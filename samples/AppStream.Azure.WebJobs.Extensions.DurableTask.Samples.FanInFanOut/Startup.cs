using AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.FanInFanOut;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.FanInFanOut
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddFanInFanOut()
                .AddTransient<IDependency, Dependency>();
        }
    }
}
