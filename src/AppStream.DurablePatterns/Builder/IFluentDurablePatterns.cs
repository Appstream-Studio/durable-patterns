using AppStream.DurablePatterns.Executor;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Builder
{
    /// <summary>
    /// Durable Functions patterns fluent API entry point.
    /// </summary>
    public interface IFluentDurablePatterns
    {
        /// <summary>
        /// Specifies the orchestration context for the Durable Function orchestration.
        /// </summary>
        /// <param name="context">The orchestration context.</param>
        /// <returns>An instance of IFluentDurablePatternsWithContext.</returns>
        IFluentDurablePatternsWithContext WithContext(IDurableOrchestrationContext context);
    }

    /// <summary>
    /// Interface for building Durable Function patterns orchestration with an orchestration context.
    /// </summary>
    public interface IFluentDurablePatternsWithContext
    {
        /// <summary>
        /// Specifies an activity to run as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <returns>An instance of IFluentDurablePatternsContinuation.</returns>
        IFluentDurablePatternsContinuation RunActivity<TActivity>()
            where TActivity : IPatternActivity;

        /// <summary>
        /// Specifies a fan-out/fan-in operation as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <param name="options">Options for the fan-out/fan-in operation.</param>
        /// <returns>An instance of IFluentDurablePatternsContinuation.</returns>
        IFluentDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options)
            where TActivity : IPatternActivity;
    }

    /// <summary>
    /// Interface for continuing to build a Durable Function patterns orchestration after specifying one or more activities as parts of the orchestration.
    /// </summary>
    public interface IFluentDurablePatternsContinuation : IFluentDurablePatternsWithContext, IExecutableDurablePatterns
    {
    }

    /// <summary>
    /// Interface for executing a built Durable Function patterns orchestration.
    /// </summary>
    public interface IExecutableDurablePatterns
    {
        /// <summary>
        /// Executes the built Durable Function patterns orchestration.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ExecutionResult> ExecuteAsync();
    }
}
