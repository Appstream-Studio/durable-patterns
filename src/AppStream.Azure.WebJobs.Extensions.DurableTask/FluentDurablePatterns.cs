using AppStream.Azure.WebJobs.Extensions.DurableTask.Executor;
using AppStream.Azure.WebJobs.Extensions.DurableTask.PatternActivity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FluentDurablePatterns : IFluentDurablePatterns, IFluentDurablePatternsWithContext
    {
        private readonly IActivityBag _activityBag;
        private readonly IStepsExecutor _stepsExecutor;
        private readonly List<Step> _steps = new();

        private IDurableOrchestrationContext? _context;

        private IDurableOrchestrationContext Context
        {
            get
            {
                if (_context == null)
                {
                    throw new ContextNotSetException();
                }

                return _context;
            }
        }

        public FluentDurablePatterns(IActivityBag activityBag, IStepsExecutor stepsExecutor)
        {
            _activityBag = activityBag;
            _stepsExecutor = stepsExecutor;
        }

        public IFluentDurablePatternsWithContext WithContext(IDurableOrchestrationContext context)
        {
            _context ??= context;
            return this;
        }

        public IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<Task<TResult>> activity)
            => RunActivityInternal<TResult>(activity);

        private IFluentDurablePatternsContinuation<TResult> RunActivityInternal<TResult>(MulticastDelegate activity)
        {
            var step = new Step(Context.NewGuid(), StepType.ActivityFunction);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsContinuation<TResult>(_activityBag, Context, _stepsExecutor, _steps);
        }

        public IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TInputItem, TResultItem>(IEnumerable<TInputItem> items, Func<IEnumerable<TInputItem>, IEnumerable<TResultItem>> activity, FanOutFanInOptions options)
            => FanOutFanInInternal<TResultItem>(activity);

        private IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanInInternal<TResultItem>(MulticastDelegate activity)
        {
            var step = new Step(Context.NewGuid(), StepType.FanOutFanIn);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsEnumerableContinuation<TResultItem>(_activityBag, Context, _stepsExecutor, _steps);
        }

        public IFluentDurablePatternsEnumerableContinuation<TResultItem> RunActivity<TResultItem>(Func<Task<IEnumerable<TResultItem>>> activity)
        {
            var step = new Step(Context.NewGuid(), StepType.ActivityFunction);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsEnumerableContinuation<TResultItem>(_activityBag, Context, _stepsExecutor, _steps);
        }

        public IFluentDurablePatternsContinuation<TResult> RunActivity<TActivity, TResult>() where TActivity : IPatternActivity<TResult>
        {
            // zapisujemy step id oraz TActivity do wora

            throw new NotImplementedException();
        }

        public IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> RunActivity<TActivity, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>
        {
            throw new NotImplementedException();
        }

        public IFluentDurablePatternsEnumerableContinuation<TResultCollectionItem> FanOutFanIn<TActivity, TResultCollection, TResultCollectionItem>()
            where TActivity : IPatternCollectionActivity<TResultCollection, TResultCollectionItem>
            where TResultCollection : IEnumerable<TResultCollectionItem>
        {
            throw new NotImplementedException();
        }
    }
}
