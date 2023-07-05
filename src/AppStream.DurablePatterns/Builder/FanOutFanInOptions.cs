namespace AppStream.DurablePatterns
{
    /// <summary>
    /// Provides options for configuring the behavior of the FanOutFanIn operation.
    /// </summary>
    /// <param name="BatchSize">Sets the maximum number of items to include in a batch. One batch is processed by one activity function.</param>
    /// <param name="ParallelActivityFunctionsCap">Sets the maximum number of parallel activity functions to execute at a time.</param>
    public record FanOutFanInOptions(int BatchSize, int ParallelActivityFunctionsCap);
}
