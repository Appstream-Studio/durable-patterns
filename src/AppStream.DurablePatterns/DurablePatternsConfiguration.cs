using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AppStream.DurablePatterns
{
    public class DurablePatternsConfiguration
    {
        private readonly IServiceCollection _services;

        public DurablePatternsConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        public void AddActivitiesFromAssembly(Assembly assembly)
        {
            var patternActivityTypes = ScanAssemblies(new[] { assembly }, typeof(IPatternActivity<,>));

            foreach (var type in patternActivityTypes)
            {
                _services.AddTransient(type);
            }
        }

        /// <summary>
        /// Scans the specified <paramref name="assemblies"/> for types that match the specified <paramref name="openGenericType"/> and returns a collection of matching types.
        /// </summary>
        /// <param name="assemblies">The collection of <see cref="Assembly"/> instances to scan.</param>
        /// <param name="openGenericType">The open generic type to match.</param>
        /// <returns>A collection of matching types.</returns>
        private static IEnumerable<Type> ScanAssemblies(Assembly[] assemblies, Type openGenericType)
        {
            var allTypes = assemblies.Distinct().SelectMany(a => a.GetTypes());

            var query = from type in allTypes
                        where !type.IsAbstract && !type.IsGenericTypeDefinition
                        let interfaces = type.GetInterfaces()
                        let genericInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                        let matchingInterface = genericInterfaces.FirstOrDefault()
                        where matchingInterface != null
                        select type;

            return query;
        }
    }
}
