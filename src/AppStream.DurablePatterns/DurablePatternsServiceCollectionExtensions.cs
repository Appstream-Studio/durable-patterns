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
    public static class DurablePatternsServiceCollectionExtensions
    {
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

        public static IServiceCollection AddDurablePatternsActivitiesFromAssembly(this IServiceCollection services, Assembly assembly) 
        {
            var patternActivityTypes = ScanAssemblies(new[] { assembly }, typeof(IPatternActivity<,>));

            foreach (var type in patternActivityTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }

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
