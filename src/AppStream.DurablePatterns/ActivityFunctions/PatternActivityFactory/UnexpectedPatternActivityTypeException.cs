namespace AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory
{
    internal class UnexpectedPatternActivityTypeException : Exception
    {
        public UnexpectedPatternActivityTypeException(Type patternActivityType, Type expectedType)
            : base($"Cannot instantiate pattern activity of type '{patternActivityType.FullName}' because is doesn't implement '{expectedType.FullName}'.")
        {
        }
    }
}
