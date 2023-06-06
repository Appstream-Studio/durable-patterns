namespace AppStream.DurablePatterns.Steps.ConfigurationValidator
{
    internal class StepValidator : IStepValidator
    {
        public void Validate(Step stepConfiguration, Step? previousStepConfiguration)
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
                throw new InvalidStepException(
                    stepConfiguration.PatternActivityType,
                    currentStepInputType,
                    previousStepConfiguration.PatternActivityType,
                    previousStepResultType);
            }
        }
    }
}
