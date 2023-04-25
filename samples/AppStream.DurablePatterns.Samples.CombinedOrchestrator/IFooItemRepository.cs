using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal interface IFooItemRepository
    {
        Task<List<FooItem>> GetFooItemsAsync();
    }
}
