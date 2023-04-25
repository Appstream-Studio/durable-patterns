namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    public class FooItem
    {
        public FooItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
