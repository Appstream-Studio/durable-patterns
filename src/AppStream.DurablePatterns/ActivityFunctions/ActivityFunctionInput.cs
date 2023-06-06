using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.ActivityFunctions
{
    internal record ActivityFunctionInput(Guid StepId, EntityId StepsEntityId, object? Input);
}
