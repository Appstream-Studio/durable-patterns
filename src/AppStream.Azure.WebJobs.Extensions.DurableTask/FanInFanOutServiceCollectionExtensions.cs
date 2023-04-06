using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker;
using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public static class FanInFanOutServiceCollectionExtensions
    {
        public static IServiceCollection AddFanInFanOut(this IServiceCollection services)
        {
            return services
                .AddSingleton<IActivityBag, ActivityBag>()
                .AddTransient<IFanInFanOut, FanInFanOut>()
                .AddTransient<IActivityInvoker, ActivityInvoker>()
                .AddTransient<IDependencyResolver, DependencyResolver>();
        }
    }
}
