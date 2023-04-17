using AppStream.Azure.WebJobs.Extensions.DurableTask.Executor;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FluentDurablePatternsEnumerableContinuation<TPreviousResultItem> : IFluentDurablePatternsEnumerableContinuation<TPreviousResultItem>
    {
        private readonly IActivityBag _activityBag;
        private readonly IDurableOrchestrationContext _context;
        private readonly IStepsExecutor _stepsExecutor;
        private readonly List<Step> _steps;

        public FluentDurablePatternsEnumerableContinuation(
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

        public IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanIn<TResultItem>(Func<IEnumerable<TPreviousResultItem>, IEnumerable<TResultItem>> activity, FanOutFanInOptions options)
            => FanOutFanInInternal<TResultItem>(activity);

        private IFluentDurablePatternsEnumerableContinuation<TResultItem> FanOutFanInInternal<TResultItem>(MulticastDelegate activity)
        {
            var step = new Step(_context.NewGuid(), StepType.FanOutFanIn);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsEnumerableContinuation<TResultItem>(_activityBag, _context, _stepsExecutor, _steps);
        }

        public IFluentDurablePatternsContinuation<TResult> RunActivity<TResult>(Func<IEnumerable<TPreviousResultItem>, TResult> activity)
            => RunActivityInternal<TResult>(activity);

        private IFluentDurablePatternsContinuation<TResult> RunActivityInternal<TResult>(MulticastDelegate activity)
        {
            var step = new Step(_context.NewGuid(), StepType.ActivityFunction);

            _steps.Add(step);
            _activityBag.Add(step.StepId, activity);

            return new FluentDurablePatternsContinuation<TResult>(_activityBag, _context, _stepsExecutor, _steps);
        }
    }
}
