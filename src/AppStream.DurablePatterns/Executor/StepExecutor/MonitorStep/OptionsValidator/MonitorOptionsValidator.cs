namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep.OptionsValidator;

internal class MonitorOptionsValidator : IMonitorOptionsValidator
{
    public void Validate<TActivityResult>(MonitorOptions? options)
    {
        if (options == null)
        {
            throw new MonitorOptionsValidationException(
                "Options cannot be null.");
        }

        if (options.PollingIntervalSeconds < 1)
        {
            throw new MonitorOptionsValidationException(
                $"{nameof(options.PollingIntervalSeconds)} must be greater than 0.");

        }

        if (options.ShouldStopFunc is not Func<TActivityResult, bool>)
        {
            throw new MonitorOptionsValidationException(
                $"{nameof(options.ShouldStopFunc)} has invalid type. Required type: {typeof(Func<TActivityResult, bool>).FullName}; Actual type: {options.ShouldStopFunc.GetType().FullName}");
        }
    }
}
