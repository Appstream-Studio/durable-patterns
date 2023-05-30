﻿using AppStream.DurablePatterns.StepsConfig;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal interface IStepExecutor
    {
        Task<StepExecutionResult> ExecuteStepAsync(
            StepConfiguration step,
            EntityId stepsConfigEntityId,
            IDurableOrchestrationContext context,
            object? input);
    }
}
