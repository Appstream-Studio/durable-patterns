using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;
using AppStream.DurablePatterns.Steps;
using AppStream.DurablePatterns.Steps.ConfigurationValidator;
using Microsoft.DurableTask;

namespace AppStream.DurablePatterns.Builder
{
    internal class DurablePatterns : IDurablePatterns, IDurablePatternsWithContext, IDurablePatternsContinuation
    {
        private readonly List<Step> _steps;
        private readonly IDurablePatternsExecutor _executor;
        private readonly IPatternActivityContractResolver _contractResolver;
        private readonly IStepValidator _stepValidator;

        private TaskOrchestrationContext? _context;

        public DurablePatterns(
            IDurablePatternsExecutor executor,
            IPatternActivityContractResolver contractResolver,
            IStepValidator stepValidator)
        {
            _steps = new();
            _executor = executor;
            _contractResolver = contractResolver;
            _stepValidator = stepValidator;
        }

        private TaskOrchestrationContext Context
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

        public Task<ExecutionResult> ExecuteAsync() 
            => _executor.ExecuteAsync(_steps, Context);

        public IDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options) where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.FanOutFanIn, options, null);

        public IDurablePatternsContinuation RunActivity<TActivity>() where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.ActivityFunction, null, null);

        public IDurablePatternsContinuation Monitor<TActivity, TActivityInput, TActivityResult>(Func<TActivityResult, bool> shouldStop, int pollingIntervalSeconds, TimeSpan expiry) where TActivity : IPatternActivity<TActivityInput, TActivityResult>
            => RunActivityInternal<TActivity>(StepType.Monitor, null, new MonitorOptions(pollingIntervalSeconds, expiry, shouldStop));

        public IDurablePatternsContinuation Monitor<TActivity, TActivityResult>(Func<TActivityResult, bool> shouldStop, int pollingIntervalSeconds, TimeSpan expiry) where TActivity : IPatternActivity<TActivityResult>
            => RunActivityInternal<TActivity>(StepType.Monitor, null, new MonitorOptions(pollingIntervalSeconds, expiry, shouldStop));

        private IDurablePatternsContinuation RunActivityInternal<TActivity>(StepType stepType, FanOutFanInOptions? fanOutFanInOptions, MonitorOptions? monitorOptions)
        {
            var stepId = Context.NewGuid();
            if (!_steps.Any(s => s.StepId == stepId))
            {
                var activityContract = _contractResolver.Resolve(typeof(TActivity));
                var stepConfiguration = new Step(
                    stepId,
                    stepType,
                    typeof(TActivity).AssemblyQualifiedName!,
                    activityContract.InputType.AssemblyQualifiedName!,
                    activityContract.ResultType.AssemblyQualifiedName!,
                    fanOutFanInOptions,
                    monitorOptions);

                _stepValidator.Validate(stepConfiguration, _steps.LastOrDefault());
                _steps.Add(stepConfiguration);
            }

            return this;
        }

        public IDurablePatternsWithContext WithContext(TaskOrchestrationContext context)
        {
            _context = context;
            return this;
        }
    }
}
