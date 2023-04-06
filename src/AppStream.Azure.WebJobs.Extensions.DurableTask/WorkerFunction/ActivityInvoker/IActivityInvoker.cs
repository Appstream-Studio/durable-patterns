namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker
{
    internal interface IActivityInvoker
    {
        Task<object?> InvokeActivityAsync(MulticastDelegate activity, IEnumerable<object> activityArg);
    }
}
