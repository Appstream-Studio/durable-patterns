using System;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.FanInFanOut
{
    internal interface IDependency
    {
        void DoStuff(FooItem item);
    }

    internal class Dependency : IDependency
    {
        public void DoStuff(FooItem item)
        {
            Console.WriteLine($"Dependent service doing stuff on item {item.Name}...");
        }
    }
}
