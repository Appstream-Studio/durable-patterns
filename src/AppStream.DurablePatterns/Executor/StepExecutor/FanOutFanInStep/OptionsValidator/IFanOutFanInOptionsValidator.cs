using AppStream.DurablePatterns.Builder;

namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator
{
    internal interface IFanOutFanInOptionsValidator
    {
        void Validate(FanOutFanInOptions? options);
    }
}
