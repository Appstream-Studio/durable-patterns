namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction
{
    internal record WorkerResult(object? ActivityResult, TimeSpan Duration);
}
