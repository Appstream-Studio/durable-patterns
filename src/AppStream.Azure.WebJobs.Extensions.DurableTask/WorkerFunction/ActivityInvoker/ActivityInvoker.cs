using AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker.DependencyResolver;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.WorkerFunction.ActivityInvoker
{
    internal class ActivityInvoker : IActivityInvoker
    {
        private readonly IDependencyResolver _dependencyResolver;

        public ActivityInvoker(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public async Task<object?> InvokeActivityAsync(MulticastDelegate activity, IEnumerable<object> activityArg)
        {
            var activityParameters = activity.Method.GetParameters();
            var convertedItems = ConvertActivityArgument(activity, activityArg, activityParameters[0]);

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
                    var resultProperty = taskType.GetProperty("Result")!;
                    activityResult = resultProperty.GetValue(task);
                }
            }

            return activityResult;
        }

        private static object ConvertActivityArgument(
            MulticastDelegate activity,
            IEnumerable<object> activityArg,
            ParameterInfo parameterInfo)
        {
            var itemType = parameterInfo.ParameterType.GetGenericArguments()[0];

            return typeof(ActivityInvoker)
                .GetMethod(nameof(CastItems))!
                .MakeGenericMethod(itemType)
                .Invoke(null, new object[] { activityArg })!;
        }

        public static IEnumerable<TItem> CastItems<TItem>(IEnumerable<object> items)
        {
            return items
                .Cast<JObject>()
                .Select(i => i.ToObject<TItem>());
        }
    }
}
