namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public class StepResult
    {
        public object? Result { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid StepId { get; set; }
        public StepType StepType { get; set; }
    }
}
