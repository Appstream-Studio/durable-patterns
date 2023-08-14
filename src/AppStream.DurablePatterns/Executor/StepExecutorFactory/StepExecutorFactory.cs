using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;
using AppStream.DurablePatterns.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.DurablePatterns.Executor.StepExecutorFactory
{
    internal class StepExecutorFactory : IStepExecutorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StepExecutorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStepExecutor Get(StepType stepType)
        {
            IStepExecutor executor = stepType switch
            {
                StepType.ActivityFunction => GetRequiredService<ActivityFunctionStepExecutor>(),
                StepType.FanOutFanIn => GetRequiredService<FanOutFanInStepExecutor>(),
                StepType.Monitor => GetRequiredService<MonitorStepExecutor>(),
                _ => throw new StepTypeNotSupportedException(stepType),
            };

            return executor;
        }

        private IStepExecutor GetRequiredService<TExecutor>()
            where TExecutor : IStepExecutor
        {
            var executor = _serviceProvider.GetService<TExecutor>() 
                ?? throw new StepExecutorNotRegisteredException(typeof(TExecutor));
 
            return executor;
        }
    }
}
