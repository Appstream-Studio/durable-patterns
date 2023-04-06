namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public record FanInFanOutResult<TBatchResult>(ActivityFunctionResult<TBatchResult?>[] Results, TimeSpan Duration);
    public record ActivityFunctionResult<TBatchResult>(TBatchResult? Result, TimeSpan Duration);
}
