using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.Steps.Entity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;

namespace AppStream.DurablePatterns.ActivityFunctions
{
    internal class ActivityFunction
    {
        public const string FunctionName = "AppStream-DurablePatterns-ActivityFunction";

        private readonly IPatternActivityFactory _patternActivityFactory;

        public ActivityFunction(
            IPatternActivityFactory patternActivityFactory)
        {
            _patternActivityFactory = patternActivityFactory;
        }

        [FunctionName(FunctionName)]
        public async Task<ActivityFunctionResult> RunWorker(
            [ActivityTrigger] IDurableActivityContext context,
            [DurableClient] IDurableEntityClient durableClient)
        {
            var functionInput = context.GetInput<ActivityFunctionInput>();
            var stepsResponse = await durableClient.ReadEntityStateAsync<StepsEntity>(functionInput.StepsEntityId);
            var stepConfiguration = stepsResponse.EntityState.Steps![functionInput.StepId];

            object? input = null;
            if (functionInput.Input != null)
            {
                var inputType = stepConfiguration.PatternActivityInputType;
                input = ((JToken)functionInput.Input).ToObject(inputType);
            }

            var task = (Task<ActivityFunctionResult>)GetType()
                .GetMethod(nameof(RunInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(stepConfiguration.PatternActivityInputType, stepConfiguration.PatternActivityResultType)
                .Invoke(this, new object?[] { stepConfiguration.PatternActivityType, input })!;

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
