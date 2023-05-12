namespace AppStream.DurablePatterns.StepsConfig.ConfigurationValidator
{
    internal interface IStepConfigurationValidator
    {
        void Validate(StepConfiguration stepConfiguration, StepConfiguration? previousStepConfiguration);
    }
}
