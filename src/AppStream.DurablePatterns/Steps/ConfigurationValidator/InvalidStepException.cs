namespace AppStream.DurablePatterns.Steps.ConfigurationValidator
{
    internal class InvalidStepException : Exception
    {
        public InvalidStepException(Type currentPatternActivityType, Type currentStepInputType, Type previousPatternActivityType, Type previousStepResultType)
            : base($"Step failed validation. Activity '{previousPatternActivityType.Name}' result type '{previousStepResultType.FullName}' cannot be assigned to next activity '{currentPatternActivityType.Name}' input type '{currentStepInputType.FullName}'.")
        {
        }
    }
}
