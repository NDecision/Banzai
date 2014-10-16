namespace Banzai
{
    /// <summary>
    /// Options that define how execution of the nodes is conducted.
    /// </summary>
    public sealed class ExecutionOptions
    {
        /// <summary>
        /// Indicates that parent processing should continue if the current node fails.
        /// </summary>
        public bool ContinueOnFailure { get; set; }

        /// <summary>
        /// Indicates that pipeline processing should throw an exception when an error occurs instead of adding it to the error collection.
        /// </summary>
        /// <remarks>
        /// This applies to the execution of the node only.  An error will always be thrown if ShouldExecute errs.
        /// </remarks>
        public bool ThrowOnError { get; set; }

        /// <summary>
        /// Sets the degree of parallelism when executing many asynchronously.
        /// </summary>
        public int DegreeOfParallelism { get; set; }
    }
}