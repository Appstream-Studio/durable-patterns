namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;

internal class MonitorExpiredException : Exception
{
    public MonitorExpiredException(TimeSpan expiry)
        : base($"Monitor expired after a set timeout of {expiry}.")
    {
    }
}
