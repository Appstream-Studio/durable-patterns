namespace AppStream.DurablePatterns.Steps
{
    public record Step(
        Guid StepId,
        StepType StepType,
        Type PatternActivityType,
        Type PatternActivityInputType,
        Type PatternActivityResultType,
        FanOutFanInOptions? FanOutFanInOptions);
}
