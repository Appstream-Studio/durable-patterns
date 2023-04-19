namespace AppStream.DurablePatterns.Builder.ContractResolver
{
    internal class PatternActivityContractException : Exception
    {
        public PatternActivityContractException(Type patternActivityType)
            : base($"Could not resolve pattern activity contract. Pattern activity type '{patternActivityType.FullName}' does not implement the generic '{typeof(IPatternActivity<,>)}' interface.")
        {
        }
    }
}
