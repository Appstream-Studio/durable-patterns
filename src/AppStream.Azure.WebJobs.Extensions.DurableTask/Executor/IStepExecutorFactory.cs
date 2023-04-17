namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal interface IStepExecutorFactory
    {
        IStepExecutor Get(StepType stepType);
    }
}
