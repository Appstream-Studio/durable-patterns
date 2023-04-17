using AppStream.Azure.WebJobs.Extensions.DurableTask.Executor;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FluentDurablePatternsContinuation<TPreviousResult> : IFluentDurablePatternsContinuation<TPreviousResult>
    {
        private readonly IActivityBag _activityBag;
        private readonly IDurableOrchestrationContext _context;
        private readonly IStepsExecutor _stepsExecutor;
        private readonly List<Step> _steps;

        public FluentDurablePatternsContinuation(
            IActivityBag activityBag, 
            IDurableOrchestrationContext context, 
            IStepsExecutor stepsExecutor,
            List<Step> steps)
        {
            _activityBag = activityBag;
            _context = context;
            _stepsExecutor = stepsExecutor;
            _steps = steps;
        }

        public Task<PatternsExecutionResult> ExecuteAsync()
        {
            return _stepsExecutor.ExecuteAsync(_steps, _context);
        }

        public IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<TPreviousResult, TResult> activity)
            => RunActivityInternal<TResult>(activity);

        private IFluentDurablePatternsContinuation<TResult> RunActivityInternal<TResult>(MulticastDelegate activity)
        {
            var step = new Step(_context.NewGuid(), StepType.ActivityFunction);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsContinuation<TResult>(_activityBag, _context, _steps);
        }

        public IFluentDurablePatternsEnumerableContinuation<TResultItem> WithEnumerableResults<TResultItem>()
        {
            if (!typeof(TPreviousResult).IsEnumerableOf(typeof(TResultItem)))
            {
                throw new InvalidWithEnumerableResultsException(
                    $"Type {typeof(TPreviousResult).FullName} must be an enumerable of items of type {typeof(TResultItem).FullName}.");
            }

            return new FluentDurablePatternsEnumerableContinuation<TResultItem>(_activityBag, _context, _steps);
        }
    }
}
