using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository
{
    internal class FooItemRepository : IFooItemRepository
    {
        public Task<List<FooItem>> GetFooItemsAsync()
        {
            return Task.FromResult(new List<FooItem>
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
