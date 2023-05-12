namespace AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory
{
    internal interface IPatternActivityFactory
    {
        IPatternActivity<TInput, TResult> Create<TInput, TResult>(Type patternActivityType);
    }
}
