namespace AppStream.DurablePatterns.Steps
{
    public record Step(
        Guid StepId,
        StepType StepType,
        string PatternActivityTypeAssemblyQualifiedName,
        string PatternActivityInputTypeAssemblyQualifiedName,
        string PatternActivityResultTypeAssemblyQualifiedName,
        FanOutFanInOptions? FanOutFanInOptions);
}
