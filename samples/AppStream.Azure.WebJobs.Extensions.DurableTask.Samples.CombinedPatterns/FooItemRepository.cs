using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal class FooItemRepository : IFooItemRepository
    {
        public Task<FooItem[]> GetFooItemsAsync()
        {
            return Task.FromResult(new[]
            {
                new FooItem("foo-item-1"),
                new FooItem("foo-item-2"),
                new FooItem("foo-item-3"),
                new FooItem("foo-item-4"),
                new FooItem("foo-item-5"),
                new FooItem("foo-item-6"),
                new FooItem("foo-item-7"),
                new FooItem("foo-item-8"),
                new FooItem("foo-item-9")
            });
        }
    }
}
