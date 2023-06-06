using AppStream.DurablePatterns.Steps;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal abstract class StepExecutorBase : IStepExecutor
    {
        protected abstract DateTime Started { get; set; }
        protected abstract StepType StepType { get; }

        protected abstract Task<StepExecutionResult> ExecuteStepInternalAsync(
            Step step,
            EntityId stepsEntityId,
            IDurableOrchestrationContext context, 
            object? input);

        public Task<StepExecutionResult> ExecuteStepAsync(
            Step step, 
            EntityId stepsEntityId, 
            IDurableOrchestrationContext context, 
            object? input)
        {
            try
            {
                return ExecuteStepInternalAsync(step, stepsEntityId, context, input);
            }
            catch (Exception e)
            {
                return Task.FromResult(
                    new StepExecutionResult(
                        null, 
                        context.CurrentUtcDateTime - Started, 
                        step.StepId, 
                        StepType, 
                        Succeeded: false, 
                        Exception: e));
            }
        }
    }
}
