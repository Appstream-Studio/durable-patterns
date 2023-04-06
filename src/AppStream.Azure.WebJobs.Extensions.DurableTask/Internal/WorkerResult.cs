namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Internal
{
    internal record WorkerResult(object? ActivityResult, TimeSpan Duration);
}
