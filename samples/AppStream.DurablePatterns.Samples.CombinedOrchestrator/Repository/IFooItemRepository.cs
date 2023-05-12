using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository
{
    internal interface IFooItemRepository
    {
        Task<List<FooItem>> GetFooItemsAsync();
    }
}
