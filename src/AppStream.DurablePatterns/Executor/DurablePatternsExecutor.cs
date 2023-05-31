using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.Steps;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor
{
    internal class DurablePatternsExecutor : IDurablePatternsExecutor
    {
        private readonly IStepExecutorFactory _stepExecutorFactory;

        public DurablePatternsExecutor(IStepExecutorFactory stepExecutorFactory)
        {
            _stepExecutorFactory = stepExecutorFactory;
        }

        public async Task<ExecutionResult> ExecuteAsync(
            IEnumerable<Step> steps,
            EntityId stepsEntityId,
            IDurableOrchestrationContext context)
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

            foreach (var step in steps)
            {
                var executor = _stepExecutorFactory.Get(step.StepType);
                object? input = null;
                if (results.TryPeek(out var previousStepResult))
                {
                    input = previousStepResult.Result;
                }

                var stepResult = await executor.ExecuteStepAsync(step, stepsEntityId, context, input);
                if (!stepResult.Succeeded)
                {
                    throw new PatternActivityFailedException(
                        step.PatternActivityType, 
                        stepResult.Exception!);
                }

                results.Push(stepResult);
            }

            var resultsSummaries = results
                .Select(r => r switch
                {
                    FanOutFanInStepExecutionResult fofi => new FanOutFanInStepExecutionResultSummary(
                        fofi.StepType.ToString(),
                        fofi.Duration,
                        fofi.Options,
                        fofi.AverageBatchProcessingDuration,
                        fofi.AverageItemProcessingDuration,
                        fofi.BatchesProcessed,
                        fofi.ItemsProcessed),

                    StepExecutionResult res => new StepExecutionResultSummary(
                        res.StepType.ToString(),
                        res.Duration)
                })
                .ToList();

            return new ExecutionResult(resultsSummaries);
        }
    }
}
