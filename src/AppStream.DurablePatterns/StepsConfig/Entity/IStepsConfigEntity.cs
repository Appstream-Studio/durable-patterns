namespace AppStream.DurablePatterns.StepsConfig.Entity
{
    public interface IStepsConfigEntity
    {
        Task Set(Dictionary<Guid, StepConfiguration> steps);
    }
}
