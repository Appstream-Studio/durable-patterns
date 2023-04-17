using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal class StepsExecutor : IStepsExecutor
    {
        private readonly IStepExecutorFactory _stepExecutorFactory;

        public StepsExecutor(IStepExecutorFactory stepExecutorFactory)
        {
            _stepExecutorFactory = stepExecutorFactory;
        }

        public async Task<PatternsExecutionResult> ExecuteAsync(IEnumerable<Step> steps, IDurableOrchestrationContext context)
        {
            var results = new Stack<StepResult>();

            foreach (var step in steps)
            {
                var stepExecutor = _stepExecutorFactory.Get(step.StepType);
                object? previousStepResult = null;
                if (results.TryPeek(out var stepResult))
                {
                    previousStepResult = stepResult.Result;
                }

                var result = await stepExecutor.ExecuteStepAsync(context, step.StepId, previousStepResult);
                results.Push(result);
            }

            return new PatternsExecutionResult();
        }
    }
}
