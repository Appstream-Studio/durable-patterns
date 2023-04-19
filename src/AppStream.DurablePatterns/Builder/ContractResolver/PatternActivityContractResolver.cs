namespace AppStream.DurablePatterns.Builder.ContractResolver
{
    internal class PatternActivityContractResolver : IPatternActivityContractResolver
    {
        public PatternActivityContract Resolve(Type patternActivityType)
        {
            var @interface = patternActivityType.GetInterface(typeof(IPatternActivity<,>).Name) 
                ?? throw new PatternActivityContractException(patternActivityType);

            var inputType = @interface.GetGenericArguments()[0];
            var resultType = @interface.GetGenericArguments()[1];

            return new PatternActivityContract(inputType, resultType);
        }
    }
}
