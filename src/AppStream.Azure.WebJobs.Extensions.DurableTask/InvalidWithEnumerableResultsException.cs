namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal class InvalidWithEnumerableResultsException : Exception
    {
        public InvalidWithEnumerableResultsException(string? message) : base(message)
        {
        }
    }
}
