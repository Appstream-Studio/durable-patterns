using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.ActivityFunctions
{
    internal record ActivityFunctionInput(Step Step, object? ActivityInput);
}
