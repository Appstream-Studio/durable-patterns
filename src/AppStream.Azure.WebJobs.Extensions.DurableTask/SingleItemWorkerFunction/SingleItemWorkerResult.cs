namespace AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction
{
    internal record SingleItemWorkerResult(object? ActivityResult, TimeSpan Duration);
}
