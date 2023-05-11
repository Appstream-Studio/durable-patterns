[![License](https://img.shields.io/badge/license-apache-green)](https://github.com/Appstream-Studio/durable-patterns/blob/main/LICENSE)

# Durable Patterns
Welcome to the Durable Patterns Library!

Our library is here to help you make better use of Azure Durable Functions Framework without unnecessary boilerplate code. It provides fluent interface for orchestrator function to easily build efficient and scalable processes that implements the typical application [patterns](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp-inproc) like:
 - fan out / fan in
 - function chaining
 
You just inject business logic enclosed in `IPatternActivity` implementations and our library will take care of creating activity functions, passing the arguments and injecting objects without the need to write repeatable sections.

## Get Started with Durable Patterns
Durable Patterns can be installed using the Nuget package manager or the `dotnet` CLI.

```
dotnet add package AppStream.DurablePatterns
```

To use this library you need to instruct durable functions framework to look for activity functions not only in your startup project by adding this property in <PropertyGroup> of your startup project's csproj:
```
<FunctionsInDependencies>true</FunctionsInDependencies>
```

### Example - fan out / fan in

#### Function Startup
```csharp
using AppStream.DurablePatterns;

[assembly: FunctionsStartup(typeof(Startup))]

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddDurablePatterns(cfg => cfg.AddActivitiesFromAssembly(typeof(MyGetItemsActivity).Assembly));
    }
}
```

#### IPatternActivity classes
```csharp
using AppStream.DurablePatterns;

internal class MyGetItemsActivity : IPatternActivity<List<Item>>
{
    private readonly IRepository _repository;

    public MyGetItemsActivity(IRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Item>> RunAsync(object? input)
    {
        return _repository.GetItemsAsync();
    }
}

internal class MyFanOutActivity : IPatternActivity<List<Item>, List<Item>>
{
    private readonly IItemProcessingService _service;

    public MyFanOutActivity(IItemProcessingService service)
    {
        _service = service;
    }

    public Task<List<Item>> RunAsync(List<Item> input)
    {
        // this block of code will be executed in parallel batches
        var processedItems = new List<Item>();

        foreach (var item in input)
        {
            processedItems.Add(_service.Process(item));
        }

        return Task.FromResult(processedItems);
    }
}
```

#### Orchestrator function
```csharp
using AppStream.DurablePatterns;

internal class MyOrchestrator
{
    private readonly IFluentDurablePatterns _patterns;

    public MyOrchestrator(
        IFluentDurablePatterns patterns)
    {
        _patterns = patterns;
    }

    [FunctionName("Orchestrator")]
    public Task<ExecutionResult> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        return _patterns
            .WithContext(context)
            .RunActivity<MyGetItemsActivity>()
            .FanOutFanIn<MyFanOutActivity>(new FanOutFanInOptions(BatchSize: 2, ParallelActivityFunctionsCap: 2))
            .ExecuteAsync();
    }
}
```

## Roadmap
- [ ] Updating orchestration status during execution
- [ ] Support for lambdas instead of explicit `IPatternActivity` implementations
- [ ] Naming the activities for easier debugging
- [ ] Add support for [Monitoring](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp-inproc#monitoring) pattern
- [ ] Add support for [Human interaction](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp-inproc#human) pattern

## Contributing
Contributions to this open source library are highly appreciated! If you're interested in helping out, please feel free to submit a pull request with your changes. We welcome contributions of all kinds, whether it's bug fixes, new features, or just improving the documentation. Please ensure that your code is well-documented, tested, and adheres to the coding conventions used in the project. Don't hesitate to reach out if you have any questions or need help getting started.
