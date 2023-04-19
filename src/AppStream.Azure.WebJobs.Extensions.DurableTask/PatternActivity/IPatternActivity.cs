namespace AppStream.Azure.WebJobs.Extensions.DurableTask.PatternActivity
{
    //public interface IPatternActivity<TResult>
    //{
    //    Task<TResult> RunAsync();
    //}

    public interface IPatternActivity<TInput, TResult>
    {
        Task<TResult> RunAsync(TInput input);
    }

    public interface IPatternActivity<TResult> : IPatternActivity<object?, TResult>
    {
    }

    //public interface IPatternCollectionActivity<TResultCollection, TResultCollectionItem>
    //    where TResultCollection : IEnumerable<TResultCollectionItem>
    //{
    //    Task<TResultCollection> RunAsync();
    //}

    //public interface IPatternCollectionActivity<TInput, TResultCollection, TResultCollectionItem>
    //    where TResultCollection : IEnumerable<TResultCollectionItem>
    //{
    //    Task<TResultCollection> RunAsync(TInput input);
    //}
}
