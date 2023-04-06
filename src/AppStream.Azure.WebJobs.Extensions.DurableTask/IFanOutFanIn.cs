using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    public interface IFanOutFanIn
    {
        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <typeparam name="TDep3">Third service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <typeparam name="TDep3">Third service dependency.</typeparam>
        /// <typeparam name="TDep4">Fourth service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <typeparam name="TDep3">Third service dependency.</typeparam>
        /// <typeparam name="TDep4">Fourth service dependency.</typeparam>
        /// <typeparam name="TDep5">Fifth service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <typeparam name="TDep3">Third service dependency.</typeparam>
        /// <typeparam name="TDep4">Fourth service dependency.</typeparam>
        /// <typeparam name="TDep5">Fifth service dependency.</typeparam>
        /// <typeparam name="TDep6">Sixth service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;

        /// <summary>
        /// Splits <paramref name="items"/> into batches and performs <paramref name="activity"/> in parallel on those chunks.
        /// </summary>
        /// <typeparam name="TItem">Type of the item to be processed in batches.</typeparam>
        /// <typeparam name="TBatchResult">Type of the batch processing result.</typeparam>
        /// <typeparam name="TDep1">First service dependency.</typeparam>
        /// <typeparam name="TDep2">Second service dependency.</typeparam>
        /// <typeparam name="TDep3">Third service dependency.</typeparam>
        /// <typeparam name="TDep4">Fourth service dependency.</typeparam>
        /// <typeparam name="TDep5">Fifth service dependency.</typeparam>
        /// <typeparam name="TDep6">Sixth service dependency.</typeparam>
        /// <typeparam name="TDep7">Seventh service dependency.</typeparam>
        /// <param name="context">Orchestration context.</param>
        /// <param name="items">Items to be batched and processed.</param>
        /// <param name="activity">Batch processing.</param>
        /// <param name="options">Fan-in-fan-out configuration.</param>
        /// <returns>Results of all ran activities.</returns>
        Task<FanOutFanInResult<TBatchResult>> FanInFanOutAsync<TItem, TBatchResult, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>(
            IDurableOrchestrationContext context,
            IEnumerable<TItem> items,
            Func<IEnumerable<TItem>, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, Task<TBatchResult>> activity,
            FanOutFanInOptions options)
            where TItem : class
            where TBatchResult : class?;
    }
}
