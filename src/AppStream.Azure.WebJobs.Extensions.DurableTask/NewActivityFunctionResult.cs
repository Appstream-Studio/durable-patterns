namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public record NewActivityFunctionResult(object? Result, TimeSpan Duration);
}
