namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;

/// <summary>
/// Options for configuring the behavior of the Monitor operation.
/// </summary>
/// <param name="PollingIntervalSeconds">Interval of monitor polling.</param>
/// <param name="Expiry">After what time the monitor should expire.</param>
/// <param name="ShouldStopFunc">Monitor stop predicate. </param>
public record MonitorOptions(
    int PollingIntervalSeconds,
    TimeSpan Expiry,
    object ShouldStopFunc);
