[![License](https://img.shields.io/badge/license-apache-green)](https://github.com/ChilliCream/graphql-platform/blob/main/LICENSE)

# Durable Patterns
Welcome to the Durable Patterns Library!

Our library is here to help you make better use of Azure Durable Functions Framework. It provides fluent interface for orchestrator function to easily build efficient and scalable processes that implements the typical application [patterns](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp-inproc) like:
 - fan out / fan in
 - function chaining
## Get Started with Durable Patterns
Durable Patterns can be installed using the Nuget package manager or the `dotnet` CLI.

```
dotnet add package DurablePatterns // TODO
```

### Example - fan out / fan in

#### Function Startup
```csharp
using AppStream.Azure.WebJobs.Extensions.DurableTask;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.FanInFanOut;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddFanInFanOut()
            .AddTransient<IDependency, Dependency>();
    }
}
```
#### Orchestrator function
```csharp
using AppStream.Azure.WebJobs.Extensions.DurableTask;

internal class FanOutFanInOrchestrator
{
    private readonly IFanOutFanIn _fanInFanOut;

    public FanOutFanInOrchestrator(
        IFanOutFanIn fanInFanOut)
    {
        _fanInFanOut = fanInFanOut;
    }

    [FunctionName("Orchestrator")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var result = await _fanInFanOut.FanInFanOutAsync(
            context: context,
            items: new[] { new FooItem("1"), new FooItem("2"), new FooItem("3"), new FooItem("4") },
            activity: (IEnumerable<FooItem> items, IDependency dependency) =>
            {
                foreach (var i in items)
                {
                    dependency.DoStuff(i);
                }
                return Task.FromResult(items.Select(i => i.Name).ToList());
            },
            options: new FanOutFanInOptions
            {
                MaxBatchSize = 2,
                MaxParallelFunctions = 2,
                UpdateOrchestrationStatus = true
            });

        Console.WriteLine("results:");
        foreach (var resultItem in result.Results)
        {
            Console.WriteLine($"duration: {resultItem.Duration} results:");
            foreach (var item in resultItem.Result)
            {
                Console.WriteLine($"\t{item}");
            }
        }
        Console.WriteLine($"total duration: {result.Duration}");
    }
}
```
