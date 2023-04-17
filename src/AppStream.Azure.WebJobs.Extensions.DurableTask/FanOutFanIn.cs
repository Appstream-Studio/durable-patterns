using AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction;
using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FanOutFanIn : IFanOutFanIn
    {
        private readonly IActivityBag _activityBag;

        public FanOutFanIn(IActivityBag activityBag)
        {
            _activityBag = activityBag;
        }

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class? => FanOutFanInInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        private async Task<FanOutFanInResult<TBatchResult>> FanOutFanInInternalAsync<TItem, TBatchResult>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            MulticastDelegate activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?
        {
            var started = context.CurrentUtcDateTime;
            var fanInFanOutId = context.NewGuid();
            _activityBag.Add(fanInFanOutId, activity);

            var batches = items.Chunk(options.MaxBatchSize);
            var results = new List<WorkerResult>();
            var waitingQueue = new Queue<TItem[]>(batches);
            var workInProgress = new List<Task<WorkerResult>>();

            var status = new OrchestrationStatus
            {
                TotalItems = items.Count(),
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
                    new SingleItemWorkerInput(fanInFanOutId, batchToProcess));

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

            var activityResults = results
                .Select(r => new ActivityFunctionResult<TBatchResult?>(
                    DeserializeResult<TBatchResult>(r.ActivityResult),
                    r.Duration))
                .ToArray();

            return new FanOutFanInResult<TBatchResult>(
                activityResults,
                finished - started);
        }

        private static TBatchResult? DeserializeResult<TBatchResult>(object? activityResult)
            where TBatchResult : class?
        {
            if (activityResult == null)
            {
                return null;
            }

            return ((JToken)activityResult).ToObject<TBatchResult>();
        }
    }
}
