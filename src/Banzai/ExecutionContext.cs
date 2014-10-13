using Banzai.Utility;

namespace Banzai
{

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

    /// <summary>
    /// This execution context is passed down the chain during execution so that child nodes can base decisions on 
    /// context created globally or mutated by previous nodes.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the nodes operate on.</typeparam>
    public sealed class ExecutionContext<T> : IExecutionContext<T>
    {
        /// <summary>
        /// Creates a new execution context.
        /// </summary>
        /// <param name="subject">Subject of the current flow.</param>
        /// <param name="globalOptions">Global options of the current flow.</param>
        /// <param name="rootResult">Root result if one has already been established.</param>
        public ExecutionContext(T subject, ExecutionOptions globalOptions = null, NodeResult rootResult = null)
        {
            State = new DynamicDictionary();
            Subject = subject;
            GlobalOptions = globalOptions ?? new ExecutionOptions();
            if (rootResult != null)
                ParentResult = rootResult;
        }

        /// <summary>
        /// Creates a child context based on the parent context.  Used for nesting multi-nodes inside of other nodes.
        /// </summary>
        /// <param name="parentContext">Parent of this context.</param>
        /// <param name="parentResult">Parent result to set on the new context, if any.</param>
        internal ExecutionContext(IExecutionContext<T> parentContext, NodeResult parentResult = null) 
        {
            Guard.AgainstNullArgument("parentContext", parentContext);
            Guard.AgainstNullArgumentProperty("parentContext", "Subject", parentContext.Subject);
            Guard.AgainstNullArgumentProperty("parentContext", "GlobalOptions", parentContext.GlobalOptions);

            Subject = parentContext.Subject;
            State = parentContext.State;
            GlobalOptions = parentContext.GlobalOptions;
            ParentResult = parentResult;
            CancelProcessing = parentContext.CancelProcessing;
        }

        /// <summary>
        /// The subject that the workflow operates on.
        /// </summary>
        public T Subject { get; private set; }

        /// <summary>
        /// A dynamic object of additional state that must be passed through the workflow.
        /// </summary>
        public dynamic State { get; private set; }

        /// <summary>
        /// The global options that this node is using for execution
        /// </summary>
        /// <remarks>This uses the global options if no node-specific options are specified.</remarks>
        public ExecutionOptions GlobalOptions { get; private set; }

        /// <summary>
        /// Rollup of this result and all results under this result
        /// </summary>
        public NodeResult ParentResult { get; private set; }

        /// <summary>
        /// Changes the current subject to the instance provided.
        /// </summary>
        /// <param name="subject">The new subject.</param>
        public void ChangeSubject(object subject)
        {
            Subject = (T) subject;
        }

        /// <summary>
        /// Cancels further processing of the flow.
        /// </summary>
        public bool CancelProcessing { get; set; }

        /// <summary>
        /// Adds a result to the execution context.
        /// </summary>
        /// <param name="result">The result to add</param>
        public void AddResult(NodeResult result)
        {
            if (ParentResult == null)
            {
                ParentResult = result;
            }
            else if(ParentResult != result)
            {
                ParentResult.AddChildResult(result);
            }
        }

    }
}