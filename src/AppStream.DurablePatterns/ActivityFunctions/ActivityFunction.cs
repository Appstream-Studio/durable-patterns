using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.StepsConfig.ConfigurationBag;
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
        private readonly IStepConfigurationBag _stepConfigurationBag;

        public ActivityFunction(
            IPatternActivityFactory patternActivityFactory,
            IStepConfigurationBag stepConfigurationBag)
        {
            _patternActivityFactory = patternActivityFactory;
            _stepConfigurationBag = stepConfigurationBag;
        }

        [FunctionName(FunctionName)]
        public Task<ActivityFunctionResult> RunWorker(
            [ActivityTrigger] IDurableActivityContext context)
        {
            var functionInput = context.GetInput<ActivityFunctionInput>();
            var stepConfiguration = _stepConfigurationBag.Get(functionInput.StepId);

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

            return task;
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
