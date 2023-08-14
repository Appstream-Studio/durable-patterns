using Microsoft.DurableTask;

namespace AppStream.DurablePatterns
{
    /// <summary>
    /// Durable Functions patterns fluent API entry point.
    /// </summary>
    public interface IDurablePatterns
    {
        /// <summary>
        /// Specifies the orchestration context for the Durable Function orchestration.
        /// </summary>
        /// <param name="context">The orchestration context.</param>
        /// <returns>An instance of <see cref="TaskOrchestrationContext"/>.</returns>
        IDurablePatternsWithContext WithContext(TaskOrchestrationContext context);
    }

    /// <summary>
    /// Interface for building Durable Function patterns orchestration with an orchestration context.
    /// </summary>
    public interface IDurablePatternsWithContext
    {
        /// <summary>
        /// Specifies an activity to run as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <returns>An instance of <see cref="IDurablePatternsContinuation"/>.</returns>
        IDurablePatternsContinuation RunActivity<TActivity>()
            where TActivity : IPatternActivity;

        /// <summary>
        /// Specifies a fan-out/fan-in operation as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <param name="options">Options for the fan-out/fan-in operation.</param>
        /// <returns>An instance of <see cref="IDurablePatternsContinuation"/>.</returns>
        IDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options)
            where TActivity : IPatternActivity;

        /// <summary>
        /// Specifies a monitoring operation as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <param name="shouldStop">Monitor stop predicate.</param>
        /// <param name="pollingIntervalSeconds">Interval of monitor polling.</param>
        /// <param name="expiry">After what time the monitor should expire.</param>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <typeparam name="TActivityInput">The type of the activity input.</typeparam>
        /// <typeparam name="TActivityResult">The type of the activity result.</typeparam>
        /// <returns>An instance of <see cref="IDurablePatternsContinuation"/>.</returns>
        IDurablePatternsContinuation Monitor<TActivity, TActivityInput, TActivityResult>(Func<TActivityResult, bool> shouldStop, int pollingIntervalSeconds, TimeSpan expiry)
            where TActivity : IPatternActivity<TActivityInput, TActivityResult>;

        /// <summary>
        /// Specifies a monitoring operation as part of the Durable Function patterns orchestration.
        /// </summary>
        /// <param name="shouldStop">Monitor stop predicate.</param>
        /// <param name="pollingIntervalSeconds">Interval of monitor polling.</param>
        /// <param name="expiry">After what time the monitor should expire.</param>
        /// <typeparam name="TActivity">The type of the activity to run.</typeparam>
        /// <typeparam name="TActivityResult">The type of the activity result.</typeparam>
        /// <returns>An instance of <see cref="IDurablePatternsContinuation"/>.</returns>
        IDurablePatternsContinuation Monitor<TActivity, TActivityResult>(Func<TActivityResult, bool> shouldStop, int pollingIntervalSeconds, TimeSpan expiry)
            where TActivity : IPatternActivity<TActivityResult>;
    }

    /// <summary>
    /// Interface for continuing to build a Durable Function patterns orchestration after specifying one or more activities as parts of the orchestration.
    /// </summary>
    public interface IDurablePatternsContinuation : IDurablePatternsWithContext, IExecutableDurablePatterns
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
