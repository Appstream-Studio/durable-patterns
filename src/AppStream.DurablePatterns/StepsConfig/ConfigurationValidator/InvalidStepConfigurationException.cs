namespace AppStream.DurablePatterns.StepsConfig.ConfigurationValidator
{
    internal class InvalidStepConfigurationException : Exception
    {
        public InvalidStepConfigurationException(Type currentPatternActivityType, Type currentStepInputType, Type previousPatternActivityType, Type previousStepResultType)
            : base($"Step configuration failed validation. Activity '{previousPatternActivityType.Name}' result type '{previousStepResultType.FullName}' cannot be assigned to next activity '{currentPatternActivityType.Name}' input type '{currentStepInputType.FullName}'.")
        {
        }
    }
}
