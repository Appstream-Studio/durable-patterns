namespace AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory
{
    internal class PatternActivityFactory : IPatternActivityFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PatternActivityFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPatternActivity<TInput, TResult> Create<TInput, TResult>(Type patternActivityType)
        {
            if (patternActivityType == null)
            {
                throw new ArgumentNullException(nameof(patternActivityType));
            }

            if (!patternActivityType.IsAssignableTo(typeof(IPatternActivity<TInput, TResult>)))
            {
                throw new UnexpectedPatternActivityTypeException(patternActivityType, typeof(IPatternActivity<TInput, TResult>));
            }

            var activity = _serviceProvider.GetService(patternActivityType) 
                ?? throw new ActivityNotRegisteredException(patternActivityType);

            return (IPatternActivity<TInput, TResult>)activity;
        }
    }
}
