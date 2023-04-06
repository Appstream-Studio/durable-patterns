using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver.Exceptions;
using System.Reflection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object[] Resolve(IEnumerable<ParameterInfo> dependencyParameters)
        {
            if (dependencyParameters == null)
            {
                throw new ArgumentNullException(nameof(dependencyParameters));
            }

            return dependencyParameters
                .Select(p => p.ParameterType)
                .Select(Resolve)
                .ToArray();
        }

        private object Resolve(Type dependencyType)
        {
            if (dependencyType == null)
            {
                throw new ArgumentNullException(nameof(dependencyType));
            }

            var dependency = _serviceProvider.GetService(dependencyType);
            return dependency ?? throw new DependencyNotFoundException(dependencyType.FullName!);
        }
    }
}
