namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class ContextNotSetException : Exception
    {
        public ContextNotSetException() 
            : base($"Orchestration context has not been set and is required. Make sure to call {nameof(IFluentDurablePatterns.WithContext)} before calling any other fluent api methods.")
        {
        }
    }
}
