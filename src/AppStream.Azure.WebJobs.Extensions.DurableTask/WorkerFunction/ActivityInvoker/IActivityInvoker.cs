namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker
{
    internal interface IActivityInvoker
    {
        Task<IEnumerable<object>> InvokeActivityAsync(MulticastDelegate activity, IEnumerable<object> activityArg);
    }
}
