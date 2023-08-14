namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep.OptionsValidator;

internal interface IMonitorOptionsValidator
{
    void Validate<TActivityResult>(MonitorOptions? options);
}
