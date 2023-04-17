namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public class FanOutFanInStepResult : StepResult
    {
        public ActivityFunctionResult[] ActivityFunctionResults { get; set; }

        public class ActivityFunctionResult
        {
            public object? Result { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
}
