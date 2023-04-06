using AppStream.Azure.WebJobs.Extensions.DurableTask.Internal;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class FanInFanOut : IFanInFanOut
    {
        private readonly IActivityBag _activityBag;

        public FanInFanOut(IActivityBag activityBag)
        {
            _activityBag = activityBag;
        }

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        public Task<FanInFanOutResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, Task<TBatchResult>> activity,
            FanInFanOutOptions options)
            where TItem : class
            where TBatchResult : class? => FanInFanOutInternalAsync<TItem, TBatchResult>(context, items, activity, options);

        private async Task<FanInFanOutResult<TBatchResult>> FanInFanOutInternalAsync<TItem, TBatchResult>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            MulticastDelegate activity,
            FanInFanOutOptions options)
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

            while (waitingQueue.Any())
            {
                var batchToProcess = waitingQueue.Dequeue();
                var task = context.CallActivityAsync<WorkerResult>(
                    WorkerFunction.FunctionName,
                    new WorkerInput(fanInFanOutId, batchToProcess));

                workInProgress.Add(task);

                if (workInProgress.Count >= options.MaxParallelFunctions)
                {
                    var completedTask = await Task.WhenAny(workInProgress);
                    var workResult = await completedTask;

                    results.Add(workResult);
                    workInProgress.Remove(completedTask);
                }
            }

            var remainingWorkResults = await Task.WhenAll(workInProgress);
            results.AddRange(remainingWorkResults);
            var finished = context.CurrentUtcDateTime;

            var activityResults = results
                .Select(r => new ActivityFunctionResult<TBatchResult?>(
                    DeserializeResult<TBatchResult>(r.ActivityResult),
                    r.Duration))
                .ToArray();

            return new FanInFanOutResult<TBatchResult>(
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
