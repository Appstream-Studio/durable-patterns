using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using Microsoft.Azure.Functions.Worker;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AppStream.DurablePatterns.ActivityFunctions
{
    internal class ActivityFunction
    {
        public const string FunctionName = "AppStream-DurablePatterns-ActivityFunction";

        private readonly IPatternActivityFactory _patternActivityFactory;

        public ActivityFunction(IPatternActivityFactory patternActivityFactory)
        {
            _patternActivityFactory = patternActivityFactory;
        }

        [Function(FunctionName)]
        public async Task<ActivityFunctionResult> RunAsync(
            [ActivityTrigger] ActivityFunctionInput input,
            FunctionContext context)
        {
            var stepConfiguration = input.Step;

            object? activityInput = null;
            if (input.ActivityInput != null)
            {
                var inputType = Type.GetType(stepConfiguration.PatternActivityInputTypeAssemblyQualifiedName)!;
                activityInput = ((JsonElement)input.ActivityInput).Deserialize(inputType);
            }

            var task = (Task<ActivityFunctionResult>)GetType()
                .GetMethod(nameof(RunInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(
                    Type.GetType(stepConfiguration.PatternActivityInputTypeAssemblyQualifiedName)!,
                    Type.GetType(stepConfiguration.PatternActivityResultTypeAssemblyQualifiedName)!)
                .Invoke(this, new object?[]
                {
                    Type.GetType(stepConfiguration.PatternActivityTypeAssemblyQualifiedName),
                    activityInput
                })!;

            return await task;
        }

        private async Task<ActivityFunctionResult> RunInternal<TActivityInput, TActivityResult>(
            Type patternActivityType,
            TActivityInput input)
        {
            var sw = Stopwatch.StartNew();
            var patternActivity = _patternActivityFactory.Create<TActivityInput, TActivityResult>(patternActivityType);
            var result = await patternActivity.RunAsync(input);

            return new ActivityFunctionResult(result!, sw.Elapsed);
        }
    }
}
