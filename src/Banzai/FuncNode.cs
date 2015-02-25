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
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        Func<IExecutionContext<T>, Task<NodeResultStatus>> ExecutedFunc { get; set; }
    }


    /// <summary>
    /// A node that exposes functions to set to perform node execution.
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public sealed class FuncNode<T> : Node<T>, IFuncNode<T>
    {

        /// <summary>
        /// Function executed when the node executes. Takes precedence over overridden PerformExecute method.
        /// </summary>
        public Func<IExecutionContext<T>, Task<NodeResultStatus>> ExecutedFunc { get; set; }

        /// <summary>
        /// Sealed method used to call the provided ExecutedFunc.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected async override Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            if (ExecutedFunc != null)
            {
                LogWriter.Debug("ExecutedFuncAsync exists, running this function.");
                return await ExecutedFunc(context).ConfigureAwait(false);
            }
            LogWriter.Debug("ExecutedFuncAsync doesn't exist, defaulting to base class PerformExecute.");
            return await base.PerformExecuteAsync(context).ConfigureAwait(false);
        }

    }
}