namespace AppStream.DurablePatterns
{
    public interface IPatternActivity
    {
    }

    public interface IPatternActivity<TInput, TResult> : IPatternActivity
    {
        Task<TResult> RunAsync(TInput input);
    }

    public interface IPatternActivity<TResult> : IPatternActivity<object?, TResult>
    {
    }
}
