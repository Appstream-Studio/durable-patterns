namespace AppStream.DurablePatterns.Steps.ConfigurationValidator
{
    internal interface IStepValidator
    {
        void Validate(Step stepConfiguration, Step? previousStepConfiguration);
    }
}
