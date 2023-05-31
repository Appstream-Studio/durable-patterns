using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.Executor.StepExecutorFactory
{
    internal interface IStepExecutorFactory
    {
        IStepExecutor Get(StepType stepType);
    }
}
