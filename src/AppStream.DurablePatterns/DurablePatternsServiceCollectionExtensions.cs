﻿using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;
using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep.OptionsValidator;
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
                .AddScoped<IDurablePatterns, Builder.DurablePatterns>()
                .AddScoped<IPatternActivityContractResolver, PatternActivityContractResolver>()
                .AddScoped<IStepValidator, StepValidator>()
                .AddScoped<IDurablePatternsExecutor, DurablePatternsExecutor>()
                .AddScoped<IStepExecutorFactory, StepExecutorFactory>()
                .AddScoped<IPatternActivityFactory, PatternActivityFactory>()
                .AddScoped<ActivityFunctionStepExecutor>()
                .AddScoped<FanOutFanInStepExecutor>()
                .AddScoped<IFanOutFanInOptionsValidator, FanOutFanInOptionsValidator>()
                .AddScoped<MonitorStepExecutor>()
                .AddScoped<IMonitorOptionsValidator, MonitorOptionsValidator>();

            configure(new DurablePatternsConfiguration(services));

            return services;
        }
    }
}
