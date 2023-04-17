using AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction;
using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal class FanOutFanInStepExecutor : IStepExecutor
    {
        public Task<StepResult> ExecuteStepAsync(
            IDurableOrchestrationContext context,
            Guid stepId,
            object? input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(
                    nameof(input), 
                    $"Fan-out-fan-in must be provided with a collection of items to work on.");
            }
            var inputType = input.GetType();
            var elementType = inputType.GetElementType();
            if (elementType == null)
            {
                throw new FanOutFanInInvalidInputTypeException(inputType);
            }

            var task = typeof(FanOutFanInStepExecutor)
                .GetMethod(nameof(ExecuteStepInternalAsync))!
                .MakeGenericMethod(elementType)
                .Invoke(this, new object?[] { context, stepId, input })!;

            return (Task<StepResult>)task;
        }

        private async Task<StepResult> ExecuteStepInternalAsync<TCollectionItem>(
            IDurableOrchestrationContext context,
            Guid stepId,
            IEnumerable<TCollectionItem> input)
        {
            var options = new FanOutFanInOptions(); // todo: to jakos przekazac by wypadao

            var started = context.CurrentUtcDateTime;
            var batches = input.Chunk(options.MaxBatchSize);
            var results = new List<WorkerResult>();
            var waitingQueue = new Queue<TCollectionItem[]>(batches);
            var workInProgress = new List<Task<WorkerResult>>();

            var status = new OrchestrationStatus
            {
                TotalItems = input.Count(),
                TotalBatches = batches.Count(),
                BatchesInQueue = waitingQueue.Count
            };
            if (!context.IsReplaying && options.UpdateOrchestrationStatus)
            {
                context.SetCustomStatus(status);
            }

            while (waitingQueue.Any())
            {
                var batchToProcess = waitingQueue.Dequeue();
                var task = context.CallActivityAsync<WorkerResult>(
                    WorkerFunction.WorkerFunction.FunctionName,
                    new SingleItemWorkerInput(stepId, batchToProcess));
                workInProgress.Add(task);

                status.BatchesInQueue = waitingQueue.Count;
                status.BatchesInProcess = workInProgress.Count;
                if (!context.IsReplaying && options.UpdateOrchestrationStatus)
                {
                    context.SetCustomStatus(status);
                }

                if (workInProgress.Count >= options.MaxParallelFunctions)
                {
                    var completedTask = await Task.WhenAny(workInProgress);
                    var workResult = await completedTask;

                    results.Add(workResult);
                    workInProgress.Remove(completedTask);

                    status.BatchesInProcess = workInProgress.Count;
                    status.BatchesProcessed = results.Count;
                    if (!context.IsReplaying && options.UpdateOrchestrationStatus)
                    {
                        context.SetCustomStatus(status);
                    }
                }
            }

            var remainingWorkResults = await Task.WhenAll(workInProgress);
            results.AddRange(remainingWorkResults);
            var finished = context.CurrentUtcDateTime;

            status.BatchesInQueue = 0;
            status.BatchesInProcess = 0;
            status.BatchesProcessed = results.Count;
            if (!context.IsReplaying && options.UpdateOrchestrationStatus)
            {
                context.SetCustomStatus(status);
            }

            return new StepResult
            {
                Duration = finished - started,
                Result = results.SelectMany(r => r.Result),
                StepId = stepId,
                StepType = StepType.FanOutFanIn
            };
        }
    }
}
