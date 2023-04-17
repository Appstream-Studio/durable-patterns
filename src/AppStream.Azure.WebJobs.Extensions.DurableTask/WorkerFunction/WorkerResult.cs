namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction
{
    internal record WorkerResult(IEnumerable<object> Result, TimeSpan Duration);
}
