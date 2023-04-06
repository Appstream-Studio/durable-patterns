namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction
{
    internal record WorkerInput(Guid ActivityId, IEnumerable<object> Items);
}
