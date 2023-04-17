namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal record Step(Guid StepId, StepType StepType);
}
