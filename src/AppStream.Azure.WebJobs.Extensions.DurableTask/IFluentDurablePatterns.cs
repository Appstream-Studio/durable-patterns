using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public interface IFluentDurablePatterns
    {
        // result determines input of the next invocation so it should be a generic type argument?
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<Task<TResult>> activity);
        #region dependencies
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult, TDep1>(Func<TDep1, Task<TResult>> activity);
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult, TDep1, TDep2>(Func<TDep1, TDep2, Task<TResult>> activity);
        // ...
        #endregion
        IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TInputItem, TResultItem>(
            IEnumerable<TInputItem> items,
            Func<IEnumerable<TInputItem>, IEnumerable<TResultItem>> activity,
            FanOutFanInOptions options);
    }

    public interface IFluentDurablePatternsContinuation<TPreviousResult> : IExecutableDurablePatterns
    {
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<TPreviousResult, TResult> activity);
        IFluentDurablePatternsEnumerableContinuation<TResultItem> WithEnumerableResults<TResultItem>(); // ???
    }

    public interface IFluentDurablePatternsEnumerableContinuation<TPreviousResultItem> : IExecutableDurablePatterns
    {
        IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TResultItem>(
            Func<IEnumerable<TPreviousResultItem>, IEnumerable<TResultItem>> activity,
            FanOutFanInOptions options);
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<IEnumerable<TPreviousResultItem>, TResult> activity);
    }

    public interface IExecutableDurablePatterns
    {
        Task<PatternsExecutionResult> ExecuteAsync(IDurableOrchestrationContext context);
    }
}
