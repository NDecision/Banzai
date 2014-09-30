using Banzai.Utility;

namespace Banzai
{
    /// <summary>
    /// This execution context is passed down the chain during execution so that child nodes can base decisions on 
    /// context created globally or mutated by previous nodes.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the nodes operate on.</typeparam>
    public class ExecutionContext<T>
    {
        private ExecutionOptions _effectiveOptions;

        public ExecutionContext(T subject, ExecutionOptions globalOptions = null, NodeResult<T> rootResult = null)
        {
            Subject = subject;
            GlobalOptions = globalOptions ?? new ExecutionOptions();
            if (rootResult != null)
                ParentResult = rootResult;
        }

        protected internal ExecutionContext(ExecutionContext<T> parentContext, NodeResult<T> parentResult = null) 
        {
            Guard.AgainstNullArgument("parentContext", parentContext);
            Guard.AgainstNullArgumentProperty("parentContext", "Subject", parentContext.Subject);
            Guard.AgainstNullArgumentProperty("parentContext", "GlobalOptions", parentContext.GlobalOptions);

            Subject = parentContext.Subject;
            GlobalOptions = parentContext.GlobalOptions;
            ParentResult = parentResult;
        }

        public T Subject { get; protected set; }

        /// <summary>
        /// The global options that this node is using for execution
        /// </summary>
        /// <remarks>This uses the global options if no node-specific options are specified.</remarks>
        public ExecutionOptions GlobalOptions { get; private set; }

        /// <summary>
        /// The effective options that this node is using for execution
        /// </summary>
        /// <remarks>Overrides global options with local node options if applicable</remarks>
        public ExecutionOptions EffectiveOptions
        {
            get
            {
                if (_effectiveOptions == null)
                    return GlobalOptions;

                return _effectiveOptions;
            }
            internal set { _effectiveOptions = value; }
        }


        /// <summary>
        /// Rollup of this result and all results under this result
        /// </summary>
        public NodeResult<T> ParentResult { get; private set; }

        /// <summary>
        /// Adds a result to the execution context.
        /// </summary>
        /// <param name="result">The result to add</param>
        internal void AddResult(NodeResult<T> result)
        {
            if (ParentResult == null)
            {
                ParentResult = result;
            }
            else
            {
                ParentResult.AddChildResult(result);
            }
            
        }

    }
}