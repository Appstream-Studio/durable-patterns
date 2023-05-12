namespace AppStream.DurablePatterns.Executor
{
    internal class PatternActivityFailedException : Exception
    {
        public PatternActivityFailedException(Type patternActivityType, Exception exception)
            : base($"Pattern activity '{patternActivityType.Name}' failed. Breaking orchestration execution. Check the inner exception for details.", exception)
        {
        }
    }
}
