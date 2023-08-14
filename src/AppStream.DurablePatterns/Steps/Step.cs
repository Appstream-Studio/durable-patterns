using AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep;
using System.Text.Json.Serialization;

namespace AppStream.DurablePatterns.Steps
{
    public class Step
    {
        public Step(
            Guid stepId,
            StepType stepType,
            string patternActivityTypeAssemblyQualifiedName,
            string patternActivityInputTypeAssemblyQualifiedName,
            string patternActivityResultTypeAssemblyQualifiedName,
            FanOutFanInOptions? fanOutFanInOptions,
            MonitorOptions? monitorStepOptions)
        {
            StepId = stepId;
            StepType = stepType;
            PatternActivityTypeAssemblyQualifiedName = patternActivityTypeAssemblyQualifiedName;
            PatternActivityInputTypeAssemblyQualifiedName = patternActivityInputTypeAssemblyQualifiedName;
            PatternActivityResultTypeAssemblyQualifiedName = patternActivityResultTypeAssemblyQualifiedName;
            FanOutFanInOptions = fanOutFanInOptions;
            MonitorStepOptions = monitorStepOptions;
        }

        public Guid StepId { get; set; }
        public StepType StepType { get; set; }
        public string PatternActivityTypeAssemblyQualifiedName { get; set; }
        public string PatternActivityInputTypeAssemblyQualifiedName { get; set; }
        public string PatternActivityResultTypeAssemblyQualifiedName { get; set; }

        [JsonIgnore]
        public FanOutFanInOptions? FanOutFanInOptions { get; set; }

        [JsonIgnore]
        public MonitorOptions? MonitorStepOptions { get; set; }
    }
}
