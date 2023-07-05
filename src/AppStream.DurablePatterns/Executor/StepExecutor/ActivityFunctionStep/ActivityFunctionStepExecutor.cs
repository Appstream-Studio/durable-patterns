using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using System.Text.Json;

namespace AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep
{
    internal class ActivityFunctionStepExecutor : StepExecutorBase
    {
        protected override DateTime Started { get; set; }
        protected override StepType StepType => StepType.ActivityFunction;

        protected override async Task<StepExecutionResult> ExecuteStepInternalAsync(
            Step step,
            TaskOrchestrationContext context,
            object? input)
        {
            Started = context.CurrentUtcDateTime;

            var result = await context.CallActivityAsync<ActivityFunctionResult>(
                ActivityFunction.FunctionName,
                new ActivityFunctionInput(step, input));

            var patternActivityResultType = Type.GetType(step.PatternActivityResultTypeAssemblyQualifiedName)!;
            var jTokenResult = (JsonElement)result.ActivityResult;
            var activityResult = jTokenResult.Deserialize(patternActivityResultType);

            return new StepExecutionResult(
                activityResult,
                context.CurrentUtcDateTime - Started,
                step.StepId,
                StepType,
                Succeeded: true,
                Exception: null);
        }
    }
}
