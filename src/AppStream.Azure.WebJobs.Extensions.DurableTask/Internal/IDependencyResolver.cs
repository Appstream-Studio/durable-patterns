using System.Reflection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Internal
{
    internal interface IDependencyResolver
    {
        object[] Resolve(IEnumerable<ParameterInfo> dependencyParameters);
    }
}
