using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker;
using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public static class FanOutFanInServiceCollectionExtensions
    {
        public static IServiceCollection AddFanInFanOut(this IServiceCollection services)
        {
            return services
                .AddSingleton<IActivityBag, ActivityBag>()
                .AddTransient<IFanOutFanIn, FanOutFanIn>()
                .AddTransient<IActivityInvoker, ActivityInvoker>()
                .AddTransient<IDependencyResolver, DependencyResolver>();
        }
    }
}
