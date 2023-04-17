namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal class FanOutFanInInvalidInputTypeException : Exception
    {
        public FanOutFanInInvalidInputTypeException(Type previousStepResultType) 
            : base($"Fan-out-fan-in input type {previousStepResultType.Name} is not an IEnumerable.")
        {
        }
    }
}
