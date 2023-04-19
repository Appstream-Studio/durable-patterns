using AppStream.DurablePatterns.Builder;

namespace AppStream.DurablePatterns.StepsConfig
{
    internal record StepConfiguration(
        Guid StepId,
        StepType StepType,
        Type PatternActivityType,
        Type PatternActivityInputType,
        Type PatternActivityResultType,
        FanOutFanInOptions? FanOutFanInOptions);
}
