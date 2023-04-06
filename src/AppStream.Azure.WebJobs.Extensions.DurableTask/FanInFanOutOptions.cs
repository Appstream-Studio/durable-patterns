﻿namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public class FanInFanOutOptions
    {
        public int MaxParallelFunctions { get; set; }
        public int MaxBatchSize { get; set; }
        public bool UpdateOrchestrationStatus { get; set; }
    }
}
