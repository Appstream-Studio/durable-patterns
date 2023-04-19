using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.StepsConfig;

namespace AppStream.DurablePatterns.Executor.StepExecutorFactory
{
    internal interface IStepExecutorFactory
    {
        IStepExecutor Get(StepType stepType);
    }
}
