namespace AppStream.DurablePatterns.Executor
{
    internal class PatternActivityFailedException : Exception
    {
        public PatternActivityFailedException(string patternActivityType, Exception exception)
            : base($"Pattern activity '{patternActivityType}' failed. Breaking orchestration execution. Check the inner exception for details.", exception)
        {
        }
    }
}
