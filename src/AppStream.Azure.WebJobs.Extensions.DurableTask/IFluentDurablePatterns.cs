using AppStream.Azure.WebJobs.Extensions.DurableTask.PatternActivity;
using DurableTask.Core;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public interface IFluentDurablePatterns
    {
        IFluentDurablePatternsWithContext WithContext(IDurableOrchestrationContext context);
    }

    public interface IFluentDurablePatternsWithContext
    {
        // result determines input of the next invocation so it should be a generic type argument?
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<Task<TResult>> activity);
        IFluentDurablePatternsEnumerableContinuation<TResultItem> RunActivity<TResultItem>(Func<Task<IEnumerable<TResultItem>>> activity);
        IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TInputItem, TResultItem>(
            IEnumerable<TInputItem> items,
            Func<IEnumerable<TInputItem>, IEnumerable<TResultItem>> activity,
            FanOutFanInOptions options);

        // IPatternActivity

        IFluentDurablePatternsContinuation<TResult> RunActivity<TActivity, TResult>()
            where TActivity : IPatternActivity<TResult>;
        IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> RunActivity<TActivity, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>;
        IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> FanOutFanIn<TActivity, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>;
    }

    public interface IFluentDurablePatternsContinuation<TPreviousResult> : IExecutableDurablePatterns
    {
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<TPreviousResult, TResult> activity);
        IFluentDurablePatternsEnumerableContinuation<TResultItem> WithEnumerableResults<TResultItem>();

        // IPatternActivity

        IFluentDurablePatternsContinuation<TResult> RunActivity<TActivity, TInput, TResult>()
            where TActivity : IPatternActivity<TInput, TResult>;
        IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> RunActivity<TActivity, TInput, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TInput, TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>;
    }

    public interface IFluentDurablePatternsEnumerableContinuation<TPreviousResultItem> : IExecutableDurablePatterns
    {
        IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TResultItem>(
            Func<IEnumerable<TPreviousResultItem>, IEnumerable<TResultItem>> activity,
            FanOutFanInOptions options);
        IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<IEnumerable<TPreviousResultItem>, TResult> activity);

        // IPatternActivity

        IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> FanOutFanIn<TActivity, TInput, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TInput, TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>;
        IFluentDurablePatternsContinuation<TResult> RunActivity<TActivity, TInput, TResult>()
            where TActivity : IPatternActivity<TInput, TResult>;
        IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> RunActivity<TActivity, TInput, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TInput, TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>;
    }

    public interface IExecutableDurablePatterns
    {
        Task<PatternsExecutionResult> ExecuteAsync();
    }
}
