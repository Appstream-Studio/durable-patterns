namespace AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory
{
    internal class ActivityNotRegisteredException : Exception
    {
        public ActivityNotRegisteredException(Type activityType)
            : base($"Cannot instantiate activity '{activityType.FullName}' because it is not registered in the service provider.")
        {
        }
    }
}
