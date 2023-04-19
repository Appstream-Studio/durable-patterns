using AppStream.DurablePatterns.Builder;

namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator
{
    internal class FanOutFanInOptionsValidator : IFanOutFanInOptionsValidator
    {
        public void Validate(FanOutFanInOptions? options)
        {
            if (options == null)
            {
                throw new FanOutFanInOptionsValidationException(
                    "Options cannot be null.");
            }

            if (options.ParallelActivityFunctionsCap < 1)
            {
                throw new FanOutFanInOptionsValidationException(
                    $"{nameof(options.ParallelActivityFunctionsCap)} must be greater than 0.");
            }

            if (options.BatchSize < 1)
            {
                throw new FanOutFanInOptionsValidationException(
                    $"{nameof(options.BatchSize)} must be greater than 0.");
            }
        }
    }
}
