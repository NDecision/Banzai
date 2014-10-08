using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// INodeSync is a convenience node to allow ShouldExecute and Execute functions to be performed without async/task semantics.
    /// </summary>
    /// <typeparam name="T">The type of the subject to be operated upon.</typeparam>
    public interface INodeSync<T> : INode<T>
    {
        /// <summary>
        /// Function to define if this node should be executed.
        /// </summary>
        Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Function to be performed when the node is executed.
        /// </summary>
        Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>Bool indicating if the current node should be run.</returns>
        bool ShouldExecute(ExecutionContext<T> context);
    }

    /// <summary>
    /// NodeSync is a convenience node to allow ShouldExecute and Execute functions to be performed without async/task semantics.
    /// </summary>
    /// <typeparam name="T">The type of the subject to be operated upon.</typeparam>
    public class NodeSync<T> : Node<T>, INodeSync<T>
    {
        /// <summary>
        /// Creates a new NodeSync.
        /// </summary>
        public NodeSync()
        {
            ShouldExecuteFunc = ShouldExecute;
            ExecutedFunc = PerformExecute;
        }

        /// <summary>
        /// Creates a new Node with local options to override global options.
        /// </summary>
        /// <param name="localOptions">Local options to override global options.</param>
        public NodeSync(ExecutionOptions localOptions): this()
        {
            LocalOptions = localOptions;
        }

        /// <summary>
        /// Synchronous function to determine if node ShouldExecute.  Takes precedence above overridden ShouldExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Synchronous function that provides functionality for the node.  Takes precedence above overridden PerformExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Determines if the current node should execute with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual bool ShouldExecute(ExecutionContext<T> context)
        {
            return true;
        }

        /// <summary>
        /// Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public override Task<bool> ShouldExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ShouldExecuteFunc(context));
        }

        /// <summary>
        /// Method to override to provide functionality to the current node with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual NodeResultStatus PerformExecute(ExecutionContext<T> context)
        {
            return NodeResultStatus.Succeeded;
        }

        /// <summary>
        /// Method to override to provide functionality to the current node.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ExecutedFunc(context));
        }
    }
}