namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep.OptionsValidator;

internal class MonitorOptionsValidationException : Exception
{
    public MonitorOptionsValidationException(string? message) : base(message)
    {
    }
}
