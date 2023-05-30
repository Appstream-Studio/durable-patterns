namespace AppStream.DurablePatterns.StepsConfig
{
    public record StepConfiguration(
        Guid StepId,
        StepType StepType,
        Type PatternActivityType,
        Type PatternActivityInputType,
        Type PatternActivityResultType,
        FanOutFanInOptions? FanOutFanInOptions);
}
