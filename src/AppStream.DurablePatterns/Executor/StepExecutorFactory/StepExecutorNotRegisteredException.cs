namespace AppStream.DurablePatterns.Executor.StepExecutorFactory
{
    internal class StepExecutorNotRegisteredException : Exception
    {
        public StepExecutorNotRegisteredException(Type executorType)
            : base($"Could not create step executor of type '{executorType.FullName}' because it is not registered in the service provider.")
        {
        }
    }
}
