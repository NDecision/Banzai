namespace Banzai.Core
{
    public class ExecutionContext<T>
    {
        public ExecutionContext(T subject, ExecutionOptions options = null)
        {
            Subject = subject;
            Options = options ?? new ExecutionOptions();
        }

        public T Subject { get; protected set; }

        /// <summary>
        /// The effective options that this node is using for execution
        /// </summary>
        /// <remarks>This uses the global options if no node-specific options are specified.</remarks>
        public ExecutionOptions Options { get; protected set; }

        /// <summary>
        /// Rollup of this result and all results under this result
        /// </summary>
        public NodeResult<T> Result { get; set; } 

    }
}