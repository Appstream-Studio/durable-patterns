using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.StepsConfig;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;

namespace AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep
{
    internal class ActivityFunctionStepExecutor : StepExecutorBase
    {
        protected override DateTime Started { get; set; }
        protected override StepType StepType => StepType.ActivityFunction;

        protected override async Task<StepExecutionResult> ExecuteStepInternalAsync(
            StepConfiguration step,
            IDurableOrchestrationContext context,
            object? input)
        {
            Started = context.CurrentUtcDateTime;

            // status?

            var result = await context.CallActivityAsync<ActivityFunctionResult>(
                ActivityFunction.FunctionName,
                new ActivityFunctionInput(step.StepId, input));

            // status?

            var jTokenResult = (JToken)result.ActivityResult;
            var activityResult = jTokenResult.ToObject(step.PatternActivityResultType);

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
