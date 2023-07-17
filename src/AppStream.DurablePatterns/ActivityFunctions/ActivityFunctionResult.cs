namespace AppStream.DurablePatterns.ActivityFunctions
{
    internal record ActivityFunctionResult(
        object ActivityResult, 
        object? Output, 
        TimeSpan Duration);
}
