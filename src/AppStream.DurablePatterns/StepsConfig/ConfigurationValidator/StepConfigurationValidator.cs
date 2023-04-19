namespace AppStream.DurablePatterns.StepsConfig.ConfigurationValidator
{
    internal class StepConfigurationValidator : IStepConfigurationValidator
    {
        public void Validate(StepConfiguration stepConfiguration, StepConfiguration? previousStepConfiguration)
        {
            if (stepConfiguration == null) 
            {
                throw new ArgumentNullException(nameof(stepConfiguration));
            }

            if (previousStepConfiguration == null)
            {
                return;
            }

            var previousStepResultType = previousStepConfiguration.PatternActivityResultType;
            var currentStepInputType = stepConfiguration.PatternActivityInputType;

            if (!previousStepResultType.IsAssignableTo(currentStepInputType))
            {
                throw new InvalidStepConfigurationException(
                    stepConfiguration.PatternActivityType, 
                    currentStepInputType, 
                    previousStepConfiguration.PatternActivityType,
                    previousStepResultType);
            }
        }
    }
}
