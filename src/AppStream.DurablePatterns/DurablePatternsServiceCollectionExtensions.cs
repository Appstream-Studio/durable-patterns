using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.Steps.ConfigurationValidator;
using Microsoft.Extensions.DependencyInjection;

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
        public static IServiceCollection AddDurablePatterns(this IServiceCollection services, Action<DurablePatternsConfiguration> configure) 
        {
            services
                .AddTransient<IDurablePatterns, Builder.DurablePatterns>()
                .AddTransient<IPatternActivityContractResolver, PatternActivityContractResolver>()
                .AddTransient<IStepValidator, StepValidator>()
                .AddTransient<IDurablePatternsExecutor, DurablePatternsExecutor>()
                .AddTransient<IStepExecutorFactory, StepExecutorFactory>()
                .AddTransient<IPatternActivityFactory, PatternActivityFactory>()
                .AddTransient<ActivityFunctionStepExecutor>()
                .AddTransient<FanOutFanInStepExecutor>()
                .AddTransient<IFanOutFanInOptionsValidator, FanOutFanInOptionsValidator>();

            configure(new DurablePatternsConfiguration(services));

            return services;
        }
    }
}
