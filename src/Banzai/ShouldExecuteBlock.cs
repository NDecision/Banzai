using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// A block for creating a small bit of reusable shouldexecute logic.
    /// </summary>
    /// <typeparam name="T">Type of the node that this block corresponds to.</typeparam>
    public interface IShouldExecuteBlock<in T>
    {
        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>Bool indicating if the current node should be run.</returns>
        Task<bool> ShouldExecuteAsync(IExecutionContext<T> context);
    }


    /// <summary>
    /// A block for creating a small bit of reusable shouldexecute logic.
    /// </summary>
    /// <typeparam name="T">Type of the node that this block corresponds to.</typeparam>
    public class ShouldExecuteBlock<T> : IShouldExecuteBlock<T>
    {

        /// <summary>
        /// Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual Task<bool> ShouldExecuteAsync(IExecutionContext<T> context)
        {
            return Task.FromResult(true);
        }
    }
}