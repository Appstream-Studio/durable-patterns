namespace AppStream.DurablePatterns.Builder
{
    internal class ContextNotSetException : Exception
    {
        public ContextNotSetException()
            : base($"Orchestration context has not been set and is required. Make sure to call {nameof(IDurablePatterns.WithContext)} before calling any other fluent api methods.")
        {
        }
    }
}
