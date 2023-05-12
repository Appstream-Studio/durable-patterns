using AppStream.DurablePatterns.StepsConfig;

namespace AppStream.DurablePatterns.Executor.StepExecutorFactory
{
    internal class StepTypeNotSupportedException : Exception
    {
        public StepTypeNotSupportedException(StepType stepType)
            : base($"Could not create step executor. Step type '{stepType}' is not supported.")
        {
        }
    }
}
