namespace Banzai
{
    /// <summary>
    /// This execution context is passed down the chain during execution so that child nodes can base decisions on 
    /// context created globally or mutated by previous nodes.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the nodes operate on.</typeparam>
    public interface IExecutionContext<out T>
    {
        /// <summary>
        /// The subject that the workflow operates on.
        /// </summary>
        T Subject { get; }

        /// <summary>
        /// A dynamic object of additional state that must be passed through the workflow.
        /// </summary>
        dynamic State { get; }

        /// <summary>
        /// The global options that this node is using for execution
        /// </summary>
        /// <remarks>This uses the global options if no node-specific options are specified.</remarks>
        ExecutionOptions GlobalOptions { get; }

        /// <summary>
        /// Rollup of this result and all results under this result
        /// </summary>
        NodeResult ParentResult { get; }

        /// <summary>
        /// Adds a result to the execution context.
        /// </summary>
        /// <param name="result">The result to add</param>
        void AddResult(NodeResult result);

        /// <summary>
        /// Changes the current subject to the instance provided.
        /// </summary>
        /// <param name="subject">The new subject.</param>
        void ChangeSubject(object subject);

        /// <summary>
        /// Cancels further processing of the flow.
        /// </summary>
        bool CancelProcessing { get; set; }

    }
}