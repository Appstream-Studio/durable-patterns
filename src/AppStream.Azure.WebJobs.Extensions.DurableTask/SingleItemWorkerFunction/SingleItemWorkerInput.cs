namespace AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction
{
    internal record SingleItemWorkerInput(Guid ActivityId, object? Input);
}
