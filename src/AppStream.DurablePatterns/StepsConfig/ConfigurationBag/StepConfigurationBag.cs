using System.Collections.Concurrent;

namespace AppStream.DurablePatterns.StepsConfig.ConfigurationBag
{
    internal class StepConfigurationBag : IStepConfigurationBag
    {
        private readonly ConcurrentDictionary<Guid, StepConfiguration> _steps;

        public StepConfigurationBag()
        {
            _steps = new ConcurrentDictionary<Guid, StepConfiguration>();
        }

        public void Add(StepConfiguration stepConfiguration)
        {
            if (stepConfiguration == null)
            {
                throw new ArgumentNullException(nameof(stepConfiguration));
            }

            _steps.TryAdd(stepConfiguration.StepId, stepConfiguration);
        }

        public StepConfiguration Get(Guid stepId)
        {
            return _steps[stepId];
        }
    }
}
