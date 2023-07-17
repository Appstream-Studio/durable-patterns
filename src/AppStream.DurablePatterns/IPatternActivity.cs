namespace AppStream.DurablePatterns
{
    /// <summary>
    /// Marker interface for pattern activity.
    /// </summary>
    public interface IPatternActivity
    {
    }

    /// <summary>
    /// Interface for pattern activity with input and result types.
    /// </summary>
    /// <typeparam name="TInput">The input type of the pattern activity.</typeparam>
    /// <typeparam name="TResult">The result type of the pattern activity.</typeparam>
    public interface IPatternActivity<TInput, TResult> : IPatternActivity
    {
        /// <summary>
        /// Runs the pattern activity.
        /// </summary>
        /// <param name="input">The input value for the pattern activity.</param>
        /// <returns>The result of the pattern activity.</returns>
        Task<PatternActivityResult<TResult>> RunAsync(TInput input);
    }

    /// <summary>
    /// Interface for pattern activity with result type only.
    /// </summary>
    /// <typeparam name="TResult">The result type of the pattern activity.</typeparam>
    public interface IPatternActivity<TResult> : IPatternActivity<object?, TResult>
    {
    }

    /// <summary>
    /// Pattern activity result object.
    /// </summary>
    /// <typeparam name="TResult">The result type of the pattern activity.</typeparam>
    public record PatternActivityResult<TResult>
    {
        public PatternActivityResult(TResult value, object? output)
        {
            Value = value;
            Output = output;
        }

        /// <summary>
        /// Result of the activity, will be passed to the next activity.
        /// </summary>
        public TResult Value { get; set; }

        /// <summary>
        /// Activity output, will be included in orchestrator output.
        /// </summary>
        public object? Output { get; set; }
    }
}
