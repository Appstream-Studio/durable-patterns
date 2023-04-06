namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Internal
{
    internal record WorkerInput(Guid ActivityId, IEnumerable<object> Items);
}
