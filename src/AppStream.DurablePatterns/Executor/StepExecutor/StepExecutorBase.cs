using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal abstract class StepExecutorBase : IStepExecutor
    {
        protected abstract DateTime Started { get; set; }
        protected abstract StepType StepType { get; }

        protected abstract Task<StepExecutionResult> ExecuteStepInternalAsync(
            Step step,
            TaskOrchestrationContext context, 
            object? input);

        public Task<StepExecutionResult> ExecuteStepAsync(
            Step step, 
            TaskOrchestrationContext context, 
            object? input)
        {
            try
            {
                return ExecuteStepInternalAsync(step, context, input);
            }
            catch (Exception e)
            {
                return Task.FromResult(
                    new StepExecutionResult(
                        step.PatternActivityTypeAssemblyQualifiedName,
                        null,
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
