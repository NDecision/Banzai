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
}