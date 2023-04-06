using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Diagnostics;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction
{
    internal class WorkerFunction
    {
        public const string FunctionName = "AppStream-Azure-WebJobs-Extensions-DurableTask-WorkerFunction";

        private readonly IActivityBag _activityBag;
        private readonly IActivityInvoker _activityInvoker;

        public WorkerFunction(
            IActivityBag activityBag, 
            IActivityInvoker activityInvoker)
        {
            _activityBag = activityBag;
            _activityInvoker = activityInvoker;
        }

        [FunctionName(FunctionName)]
        public async Task<WorkerResult> RunWorker(
            [ActivityTrigger] IDurableActivityContext context)
        {
            var sw = Stopwatch.StartNew();
            var input = context.GetInput<WorkerInput>();

            var activity = _activityBag.Get(input.ActivityId);
            var activityResult = await _activityInvoker.InvokeActivityAsync(activity, input.Items);

            return new WorkerResult(activityResult, sw.Elapsed);
        }
    }
}
