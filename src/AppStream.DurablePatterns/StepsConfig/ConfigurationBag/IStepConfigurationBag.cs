namespace AppStream.DurablePatterns.StepsConfig.ConfigurationBag
{
    internal interface IStepConfigurationBag
    {
        void Add(StepConfiguration stepConfiguration);
        StepConfiguration Get(Guid stepId);
    }
}
