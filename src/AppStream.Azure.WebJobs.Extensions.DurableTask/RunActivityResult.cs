namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public record RunActivityResult<TResult>(TResult Result, TimeSpan Duration);
}
