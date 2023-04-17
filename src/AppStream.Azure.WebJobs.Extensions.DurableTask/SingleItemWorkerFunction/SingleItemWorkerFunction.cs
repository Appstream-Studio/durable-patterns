using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Diagnostics;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction
{   
    internal class SingleItemWorkerFunction
    {
        public const string FunctionName = "AppStream-Azure-WebJobs-Extensions-DurableTask-SingleItemWorkerFunction";

        private readonly IActivityBag _activityBag;
        private readonly IActivityInvoker _activityInvoker;

        public SingleItemWorkerFunction(
            IActivityBag activityBag,
            IActivityInvoker activityInvoker)
        {
            _activityBag = activityBag;
            _activityInvoker = activityInvoker;
        }

        [FunctionName(FunctionName)]
        public async Task<SingleItemWorkerResult> RunWorker(
            [ActivityTrigger] IDurableActivityContext context)
        {
            var sw = Stopwatch.StartNew();
            var input = context.GetInput<SingleItemWorkerInput>();

            var activity = _activityBag.Get(input.ActivityId);
            var activityResult = await _activityInvoker.InvokeActivityAsync(activity, input.Items);

            return new SingleItemWorkerResult(activityResult, sw.Elapsed);
        }
    }
}
