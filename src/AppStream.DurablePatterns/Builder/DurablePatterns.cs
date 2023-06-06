using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Steps;
using AppStream.DurablePatterns.Steps.ConfigurationValidator;
using AppStream.DurablePatterns.Steps.Entity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Builder
{
    internal class DurablePatterns : IDurablePatterns, IDurablePatternsWithContext, IDurablePatternsContinuation
    {
        private readonly List<Step> _steps;
        private readonly IDurablePatternsExecutor _executor;
        private readonly IPatternActivityContractResolver _contractResolver;
        private readonly IStepValidator _stepValidator;

        private IDurableOrchestrationContext? _context;

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

        public async Task<ExecutionResult> ExecuteAsync()
        {
            var stepsEntityId = new EntityId(
                nameof(StepsEntity),
                Context.NewGuid().ToString());

            var proxy = Context.CreateEntityProxy<IStepsEntity>(stepsEntityId);
            var stepsDict = _steps.ToDictionary(s => s.StepId, s => s);
            await proxy.Set(stepsDict);

            return await _executor.ExecuteAsync(
                _steps,
                stepsEntityId,
                Context);
        }

        public IDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options) where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.FanOutFanIn, options);

        public IDurablePatternsContinuation RunActivity<TActivity>() where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.ActivityFunction, null);

        private IDurablePatternsContinuation RunActivityInternal<TActivity>(StepType stepType, FanOutFanInOptions? options)
        {
            var stepId = Context.NewGuid();
            if (!_steps.Any(s => s.StepId == stepId))
            {
                var activityContract = _contractResolver.Resolve(typeof(TActivity));
                var stepConfiguration = new Step(
                    stepId,
                    stepType,
                    typeof(TActivity),
                    activityContract.InputType,
                    activityContract.ResultType,
                    options);

                _stepValidator.Validate(stepConfiguration, _steps.LastOrDefault());
                _steps.Add(stepConfiguration);
            }

            return this;
        }

        public IDurablePatternsWithContext WithContext(IDurableOrchestrationContext context)
        {
            _context = context;
            return this;
        }
    }
}
