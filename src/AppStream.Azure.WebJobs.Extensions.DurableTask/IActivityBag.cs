namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal interface IActivityBag
    {
        void Add(Guid activityId, MulticastDelegate activity);
        MulticastDelegate Get(Guid activityId);
    }
}
