using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.StepsConfig;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep
{
    internal class FanOutFanInStepExecutor : StepExecutorBase
    {
        private readonly IFanOutFanInOptionsValidator _optionsValidator;

        public FanOutFanInStepExecutor(IFanOutFanInOptionsValidator optionsValidator)
        {
            _optionsValidator = optionsValidator;
        }

        protected override DateTime Started { get; set; }
        protected override StepType StepType => StepType.FanOutFanIn;

        protected override async Task<StepExecutionResult> ExecuteStepInternalAsync(
            StepConfiguration step,
            EntityId stepsConfigEntityId,
            IDurableOrchestrationContext context,
            object? input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!input.GetType().IsGenericCollection())
            {
                throw new ArgumentException(
                    $"Cannot execute FanOutFanIn step. Input type '{input.GetType().FullName}' has to implement '{typeof(ICollection<>).FullName}' interface and cannot be an array.",
                    nameof(input));
            }

            if (!step.PatternActivityResultType.IsGenericCollection())
            {
                throw new ArgumentException(
                    $"Cannot execute FanOutFanIn step. Result type '{step.PatternActivityResultType.FullName}' has to implement '{typeof(ICollection<>).FullName}' interface and cannot be an array.",
                    nameof(step));
            }

            _optionsValidator.Validate(step.FanOutFanInOptions);

            var result = await (Task<FanOutFanInStepExecutionResult>)GetType()
                .GetMethod(nameof(ExecuteFanOutFanInInternalAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(
                    input.GetType(),
                    input.GetType().GetCollectionElementType()!,
                    step.PatternActivityResultType,
                    step.PatternActivityResultType.GetCollectionElementType()!)
                .Invoke(this, new object?[] { step, stepsConfigEntityId, context, input })!;

            return result;
        }

        private async Task<FanOutFanInStepExecutionResult> ExecuteFanOutFanInInternalAsync<TInputCollection, TInputItem, TResultCollection, TResultItem>(
            StepConfiguration step,
            EntityId stepsConfigEntityId,
            IDurableOrchestrationContext context,
            TInputCollection input)
            where TInputCollection : ICollection<TInputItem>
            where TResultCollection : ICollection<TResultItem>
        {
            Started = context.CurrentUtcDateTime;
            var options = step.FanOutFanInOptions!;

            var batches = input.Chunk(options.BatchSize);

            var results = new List<ActivityFunctionResult>();
            var workQueue = new Queue<TInputItem[]>(batches);
            var workInProgress = new List<Task<ActivityFunctionResult>>();

            // toodo: status

            while (workQueue.Any())
            {
                var batchToProcess = workQueue.Dequeue();
                var activityInput = Activator.CreateInstance<TInputCollection>();
                foreach (var item in batchToProcess)
                {
                    activityInput.Add(item);
                }

                var task = context.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    new ActivityFunctionInput(step.StepId, stepsConfigEntityId, activityInput));

                workInProgress.Add(task);

                if (workInProgress.Count >= options.ParallelActivityFunctionsCap)
                {
                    var completedTask = await Task.WhenAny(workInProgress);
                    var workResult = await completedTask;

                    results.Add(workResult);
                    workInProgress.Remove(completedTask);
                }
            }

            var remainingWorkResults = await Task.WhenAll(workInProgress);
            results.AddRange(remainingWorkResults);

            var combinedResults = CombineResults<TResultCollection, TResultItem>(results);
            var averageBatchDuration = TimeSpan.FromMilliseconds(results.Average(r => r.Duration.Milliseconds));
            return new FanOutFanInStepExecutionResult(
                combinedResults,
                context.CurrentUtcDateTime - Started,
                step.StepId,
                StepType,
                Succeeded: true,
                Exception: null,
                Options: options,
                AverageBatchProcessingDuration: averageBatchDuration,
                AverageItemProcessingDuration: averageBatchDuration / options.BatchSize,
                BatchesProcessed: batches.Count(),
                ItemsProcessed: input.Count);
        }

        private static TResultCollection CombineResults<TResultCollection, TResultItem>(IEnumerable<ActivityFunctionResult> results)
            where TResultCollection : ICollection<TResultItem>
        {
            var combinedResults = Activator.CreateInstance<TResultCollection>();
            var deserializedResults = results
                .Select(r => r.ActivityResult)
                .Cast<JToken>()
                .Select(t => t.ToObject<TResultCollection>())
                .SelectMany(r => r!);

            foreach (var resultItem in deserializedResults)
            {
                combinedResults.Add(resultItem);
            }

            return combinedResults;
        }
    }
}
