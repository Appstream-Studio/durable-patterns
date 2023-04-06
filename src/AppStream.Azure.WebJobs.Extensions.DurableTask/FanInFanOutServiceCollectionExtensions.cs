using AppStream.Azure.WebJobs.Extensions.DurableTask.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public static class FanInFanOutServiceCollectionExtensions
    {
        public static IServiceCollection AddFanInFanOut(this IServiceCollection services)
        {
            return services
                .AddSingleton<IFanInFanOut, FanInFanOut>()
                .AddSingleton<IActivityBag, ActivityBag>()
                .AddTransient<IDependencyResolver, DependencyResolver>();
        }
    }
}
