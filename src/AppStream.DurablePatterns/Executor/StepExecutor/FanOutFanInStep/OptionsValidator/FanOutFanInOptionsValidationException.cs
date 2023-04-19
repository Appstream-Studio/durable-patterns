namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator
{
    internal class FanOutFanInOptionsValidationException : Exception
    {
        public FanOutFanInOptionsValidationException(string? message) : base(message)
        {
        }
    }
}
