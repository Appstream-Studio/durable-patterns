using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.StepsConfig.ConfigurationBag;
using AppStream.DurablePatterns.StepsConfig.ConfigurationValidator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AppStream.DurablePatterns
{
    /// <summary>
    /// Extension methods for registering required dependencies of the durable patterns library.
    /// </summary>
    public static class DurablePatternsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all required services for the durable patterns library to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> with the newly added services.</returns>
        public static IServiceCollection AddDurablePatterns(this IServiceCollection services) 
        {
            return services
                .AddSingleton<IFluentDurablePatterns, FluentDurablePatterns>()
                .AddSingleton<IPatternActivityContractResolver, PatternActivityContractResolver>()
                .AddSingleton<IStepConfigurationValidator, StepConfigurationValidator>()
                .AddSingleton<IStepConfigurationBag, StepConfigurationBag>()
                .AddSingleton<IFluentDurablePatternsExecutor, FluentDurablePatternsExecutor>()
                .AddSingleton<IStepExecutorFactory, StepExecutorFactory>()
                .AddSingleton<IPatternActivityFactory, PatternActivityFactory>()
                .AddSingleton<ActivityFunctionStepExecutor>()
                .AddSingleton<FanOutFanInStepExecutor>()
                .AddSingleton<IFanOutFanInOptionsValidator, FanOutFanInOptionsValidator>();
        }

        /// <summary>
        /// Scans the specified <paramref name="assembly"/> for implementations of the <see cref="IPatternActivity{TInput, TResult}"/> interface and adds them to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to scan for implementations of the <see cref="IPatternActivity{TInput, TResult}"/> interface.</param>
        /// <returns>The <see cref="IServiceCollection"/> with the newly added services.</returns>
        public static IServiceCollection AddDurablePatternsActivitiesFromAssembly(this IServiceCollection services, Assembly assembly) 
        {
            var patternActivityTypes = ScanAssemblies(new[] { assembly }, typeof(IPatternActivity<,>));

            foreach (var type in patternActivityTypes)
            {
                services.AddTransient(type);
            }

            return services;
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
