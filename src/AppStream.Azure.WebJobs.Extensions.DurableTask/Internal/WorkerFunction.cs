using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Internal
{
    internal class WorkerFunction
    {
        public const string FunctionName = "WorkerFunction";

        private readonly IActivityBag _activityBag;
        private readonly IDependencyResolver _dependencyResolver;

        public WorkerFunction(IActivityBag activityBag, IDependencyResolver dependencyResolver)
        {
            _activityBag = activityBag;
            _dependencyResolver = dependencyResolver;
        }

        [FunctionName(FunctionName)]
        public async Task<WorkerResult> RunWorker(
            [ActivityTrigger] IDurableActivityContext context)
        {
            var sw = Stopwatch.StartNew();
            var input = context.GetInput<WorkerInput>();
            var activity = _activityBag.Get(input.ActivityId);

            var activityParameters = activity.Method.GetParameters();
            var itemCollectionType = activityParameters[0].ParameterType;
            var itemType = itemCollectionType.GetGenericArguments()[0];

            var convertedItems = typeof(WorkerFunction)
                .GetMethod(nameof(CastItems))!
                .MakeGenericMethod(itemType)
                .Invoke(null, new object[] { input.Items })!;

            // todo: co jeśli nie ma dependencies?
            var dependencyParameters = activityParameters[1..];
            var dependencies = _dependencyResolver.Resolve(dependencyParameters);

            var activityArgs = new object[dependencies.Length + 1];
            activityArgs[0] = convertedItems;
            for (int i = 0; i < dependencies.Length; i++)
            {
                activityArgs[i + 1] = dependencies[i];
            }

            var activityResult = activity.DynamicInvoke(activityArgs);
            if (activityResult is Task task)
            {
                await task;

                var taskType = task.GetType();
                if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = taskType.GetGenericArguments()[0];
                    var resultProperty = taskType.GetProperty("Result")!;
                    activityResult = resultProperty.GetValue(task);
                }
            }

            return new WorkerResult(activityResult, sw.Elapsed);
        }

        public static IEnumerable<TItem> CastItems<TItem>(IEnumerable<object> items)
        {
            return items
                .Cast<JObject>()
                .Select(i => i.ToObject<TItem>());
        }
    }
}
