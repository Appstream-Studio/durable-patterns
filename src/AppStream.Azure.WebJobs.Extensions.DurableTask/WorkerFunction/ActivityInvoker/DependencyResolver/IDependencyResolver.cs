using System.Reflection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver
{
    internal interface IDependencyResolver
    {
        object[] Resolve(IEnumerable<ParameterInfo> dependencyParameters);
    }
}
