using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep.OptionsValidator;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using System.Reflection;
using System.Text.Json;

namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;

internal class MonitorStepExecutor : StepExecutorBase
{
    private readonly IMonitorOptionsValidator _optionsValidator;

    public MonitorStepExecutor(IMonitorOptionsValidator optionsValidator)
    {
        _optionsValidator = optionsValidator;
    }

    protected override DateTime Started { get; set; }

    protected override StepType StepType => StepType.Monitor;

    protected override async Task<StepExecutionResult> ExecuteStepInternalAsync(Step step, TaskOrchestrationContext context, object? input)
    {
        var patternActivityResultType = Type.GetType(step.PatternActivityResultTypeAssemblyQualifiedName)!;
        var result = await (Task<MonitorStepExecutionResult>)GetType()
            .GetMethod(nameof(ExecuteMonitorInternalAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(patternActivityResultType)
            .Invoke(this, new object?[] { step, context, input })!;

        return result;
    }

    private async Task<MonitorStepExecutionResult> ExecuteMonitorInternalAsync<TActivityResult>(
        Step step,
        TaskOrchestrationContext context,
        object? input)
    {
        Started = context.CurrentUtcDateTime;
        var pollsMade = 0;

        var options = step.MonitorStepOptions!;
        _optionsValidator.Validate<TActivityResult>(options);

        var expiry = options.Expiry;
        var expiryTime = context.CurrentUtcDateTime.Add(expiry);

        var shouldStop = (Func<TActivityResult, bool>)options.ShouldStopFunc;

        while (context.CurrentUtcDateTime < expiryTime)
        {
            var result = await context.CallActivityAsync<ActivityFunctionResult>(
                ActivityFunction.FunctionName,
                new ActivityFunctionInput(step, input));

            var jTokenResult = (JsonElement)result.ActivityResult;
            var activityResult = (TActivityResult)jTokenResult.Deserialize(typeof(TActivityResult))!;
            pollsMade++;

            if (shouldStop(activityResult))
            {
                return new MonitorStepExecutionResult(
                    step.PatternActivityTypeAssemblyQualifiedName,
                    activityResult,
                    result.Output,
                    context.CurrentUtcDateTime - Started,
                    step.StepId,
                    StepType,
                    Succeeded: true,
                    Exception: null,
                    Expiry: expiry,
                    PollsMade: pollsMade,
                    PollingInterval: options.PollingIntervalSeconds);
            }

            var nextCheck = context.CurrentUtcDateTime.AddSeconds(options.PollingIntervalSeconds);
            await context.CreateTimer(nextCheck, CancellationToken.None);
        }

        throw new MonitorExpiredException(expiry);
    }
}
