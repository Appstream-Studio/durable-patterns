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

            var previousStepResultType = Type.GetType(previousStepConfiguration.PatternActivityResultTypeAssemblyQualifiedName)!;
            var currentStepInputType = Type.GetType(stepConfiguration.PatternActivityInputTypeAssemblyQualifiedName)!;

            if (!previousStepResultType.IsAssignableTo(currentStepInputType))
            {
                var patternActivityType = Type.GetType(stepConfiguration.PatternActivityTypeAssemblyQualifiedName)!;
                var previousPatternActivityType = Type.GetType(previousStepConfiguration.PatternActivityTypeAssemblyQualifiedName)!;

                throw new InvalidStepException(
                    patternActivityType,
                    currentStepInputType,
                    previousPatternActivityType,
                    previousStepResultType);
            }
        }
    }
}
