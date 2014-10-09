using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// A node that exposes functions to set to perform node execution.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public interface IFuncNode<T> : INode<T>
    {
        /// <summary>
        /// Function to be performed when the node is executed.
        /// </summary>
        Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecutedFuncAsync { get; set; }
    }


    /// <summary>
    /// A node that exposes functions to set to perform node execution.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public sealed class FuncNode<T> : Node<T>, IFuncNode<T>
    {
        /// <summary>
        /// Synchronous function that provides functionality for the node.  Takes precedence above overridden PerformExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Function executed when the node executes. Takes precedence over overridden PerformExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecutedFuncAsync { get; set; }

        /// <summary>
        /// Sealed method used to call the provided ExecutedFunc.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected async override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            if (ExecutedFuncAsync != null)
            {
                LogWriter.Debug("ExecutedFuncAsync exists, running this function.");
                return await ExecutedFuncAsync(context).ConfigureAwait(false);
            }
            LogWriter.Debug("ExecutedFuncAsync doesn't exist.");
            return PerformExecute(context);
        }

        /// <summary>
        /// Sealed method used to call the provided ExecutedFunc.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected override NodeResultStatus PerformExecute(ExecutionContext<T> context)
        {
            if (ExecutedFunc != null)
            {
                LogWriter.Debug("ExecutedFuncAsync exists, running this function.");
                return ExecutedFunc(context);
            }
            LogWriter.Debug("ExecutedFuncAsync doesn't exist, defaulting to base class PerformExecute.");
            return base.PerformExecute(context);
        }
    }
}