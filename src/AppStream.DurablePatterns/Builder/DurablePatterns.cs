﻿using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.StepsConfig;
using AppStream.DurablePatterns.StepsConfig.ConfigurationBag;
using AppStream.DurablePatterns.StepsConfig.ConfigurationValidator;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Builder
{
    internal class DurablePatterns : IDurablePatterns, IDurablePatternsWithContext, IDurablePatternsContinuation
    {
        private readonly List<StepConfiguration> _steps;
        private readonly IDurablePatternsExecutor _executor;
        private readonly IPatternActivityContractResolver _contractResolver;
        private readonly IStepConfigurationBag _stepConfigurationBag;
        private readonly IStepConfigurationValidator _stepValidator;

        private IDurableOrchestrationContext? _context;

        public DurablePatterns(
            IDurablePatternsExecutor executor,
            IPatternActivityContractResolver contractResolver,
            IStepConfigurationBag stepConfigurationBag,
            IStepConfigurationValidator stepValidator)
        {
            _steps = new();
            _executor = executor;
            _contractResolver = contractResolver;
            _stepConfigurationBag = stepConfigurationBag;
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

        public Task<ExecutionResult> ExecuteAsync()
            => _executor.ExecuteAsync(_steps, Context);

        public IDurablePatternsContinuation FanOutFanIn<TActivity>(FanOutFanInOptions options) where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.FanOutFanIn, options);

        public IDurablePatternsContinuation RunActivity<TActivity>() where TActivity : IPatternActivity
            => RunActivityInternal<TActivity>(StepType.ActivityFunction, null);

        private IDurablePatternsContinuation RunActivityInternal<TActivity>(StepType stepType, FanOutFanInOptions? options)
        {
            if (!Context.IsReplaying)
            {
                var activityContract = _contractResolver.Resolve(typeof(TActivity));
                var stepConfiguration = new StepConfiguration(
                    Context.NewGuid(),
                    stepType,
                    typeof(TActivity),
                    activityContract.InputType,
                    activityContract.ResultType,
                    options);

                _stepValidator.Validate(stepConfiguration, _steps.LastOrDefault());
                _steps.Add(stepConfiguration);
                _stepConfigurationBag.Add(stepConfiguration);
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
