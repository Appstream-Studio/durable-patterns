using System.Collections.Concurrent;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class ActivityBag : IActivityBag
    {
        private readonly ConcurrentDictionary<Guid, MulticastDelegate> _activities;

        public ActivityBag()
        {
            _activities = new ConcurrentDictionary<Guid, MulticastDelegate>();
        }

        public void Add(Guid activityId, MulticastDelegate activity)
        {
            _activities.TryAdd(activityId, activity);
        }

        public MulticastDelegate Get(Guid activityId)
        {
            return _activities[activityId];
        }
    }
}
