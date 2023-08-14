using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace AppStream.DurablePatterns.Executor
{
    internal class DurablePatternsExecutor : IDurablePatternsExecutor
    {
        private readonly ILogger<DurablePatternsExecutor> _logger;
        private readonly IStepExecutorFactory _stepExecutorFactory;

        public DurablePatternsExecutor(
            ILogger<DurablePatternsExecutor> logger,
            IStepExecutorFactory stepExecutorFactory)
        {
            _logger = logger;
            _stepExecutorFactory = stepExecutorFactory;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            IEnumerable<Step> steps,
            TaskOrchestrationContext context)
        {
            try
            {
                if (steps == null)
                {
                    throw new ArgumentNullException(nameof(steps));
                }

                if (!steps.Any())
                {
                    throw new ArgumentException("Steps collection cannot be empty.", nameof(steps));
                }

                var results = new Stack<StepExecutionResult>();
                var outputs = new List<StepExecutionResultSummary>();

                foreach (var step in steps)
                {
                    var executor = _stepExecutorFactory.Get(step.StepType);
                    object? input = null;
                    if (results.TryPeek(out var previousStepResult))
                    {
                        input = previousStepResult.Result;
                    }

                    var stepResult = await executor.ExecuteStepAsync(step, context, input);
                    if (!stepResult.Succeeded)
                    {
                        throw new PatternActivityFailedException(
                            step.PatternActivityTypeAssemblyQualifiedName,
                            stepResult.Exception!);
                    }

                    results.Push(stepResult);
                    outputs.Add(ResultToOutput(stepResult));

                    context.SetCustomStatus(outputs);
                }

                context.SetCustomStatus(null);
                return new ExecutionResult(outputs);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unhandled exception while executing durable patterns");
                throw;
            }
        }

        private static StepExecutionResultSummary ResultToOutput(StepExecutionResult r)
        {
            return r switch
            {
                FanOutFanInStepExecutionResult res => new FanOutFanInStepExecutionResultSummary(
                    res.StepType.ToString(),
                    res.PatternActivityType,
                    res.Output,
                    res.Duration,
                    res.Options,
                    res.AverageBatchProcessingDuration,
                    res.AverageItemProcessingDuration,
                    res.BatchesProcessed,
                    res.ItemsProcessed),

                StepExecutionResult res => new StepExecutionResultSummary(
                    res.StepType.ToString(),
                    res.PatternActivityType,
                    res.Output,
                    res.Duration)
            };
        }
    }
}
