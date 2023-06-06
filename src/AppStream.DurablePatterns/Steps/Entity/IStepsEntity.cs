namespace AppStream.DurablePatterns.Steps.Entity
{
    public interface IStepsEntity
    {
        Task Set(Dictionary<Guid, Step> steps);
    }
}
