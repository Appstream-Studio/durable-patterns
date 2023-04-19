namespace AppStream.DurablePatterns.Builder.ContractResolver
{
    internal record PatternActivityContract(Type InputType, Type ResultType);

    internal interface IPatternActivityContractResolver
    {
        PatternActivityContract Resolve(Type patternActivityType);
    }
}
