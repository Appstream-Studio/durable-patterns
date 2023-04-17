namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public class ActivityFunctionStepResult : StepResult
    {
        public object? ActivityFunctionResult { get; set; }
    }
}
