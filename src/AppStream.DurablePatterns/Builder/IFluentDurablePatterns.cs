using AppStream.DurablePatterns.Executor;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Builder
{
    public interface IFluentDurablePatterns
    {
        IFluentDurablePatternsWithContext WithContext(IDurableOrchestrationContext context);
    }

    public interface IFluentDurablePatternsWithContext
    {
        IFluentDurablePatternsContinuation RunActivity<TActivity>()
            where TActivity : IPatternActivity;
        IFluentDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options)
            where TActivity : IPatternActivity;
    }

    public interface IFluentDurablePatternsContinuation : IFluentDurablePatternsWithContext, IExecutableDurablePatterns
    {
    }

    public interface IExecutableDurablePatterns
    {
        Task<ExecutionResult> ExecuteAsync();
    }
}
