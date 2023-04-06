namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Internal.Exceptions
{
    internal class DependencyNotFoundException : Exception
    {
        public DependencyNotFoundException(string dependencyTypeFullName) 
            : base($"Dependency {dependencyTypeFullName} not found in registered services. Register your services in the service collection.")
        {
        }
    }
}
