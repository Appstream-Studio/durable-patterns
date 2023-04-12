namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FluentDurablePatterns : IFluentDurablePatterns
    {
        private readonly IActivityBag _activityBag;

        public FluentDurablePatterns(IActivityBag activityBag)
        {
            _activityBag = activityBag;
        }

        public IFluentDurablePatternsEnumerableContinuation FanOutFanIn<TInputItem, TResultItem>(IEnumerable<TInputItem> items, Func<IEnumerable<TInputItem>, IEnumerable<TResultItem>> activity, FanOutFanInOptions options)
        {
            throw new NotImplementedException();
        }

        public IFluentDurablePatternsContinuation RunActivity<TResult>(Func<Task<TResult>> activity)
        {
            // jak będzie wyglądać konfiguracja całego flow w pamięci?
            /*
             *  {
             *      steps: [
             *          {
             *              stepId: guid, // activity is saved to the bag with the same id
             *              stepType: enum: ActivityFunction, FanOutFanIn
             *          }
             *      ]
             *  }
             */
        }

        public IFluentDurablePatternsContinuation RunActivity<TResult, TDep1>(Func<TDep1, Task<TResult>> activity)
        {
            throw new NotImplementedException();
        }

        public IFluentDurablePatternsContinuation RunActivity<TResult, TDep1, TDep2>(Func<TDep1, TDep2, Task<TResult>> activity)
        {
            throw new NotImplementedException();
        }
    }
}
