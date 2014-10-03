using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// NodeSync is a convenience node to allow ShouldExecute and Execute functions to be performed without async/task semantics.
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


    public class NodeSync<T> : Node<T>, INodeSync<T>
    {
        public NodeSync()
        {
            ShouldExecuteFunc = ShouldExecute;
            ExecutedFunc = PerformExecute;
        }

        public NodeSync(ExecutionOptions localOptions): this()
        {
            LocalOptions = localOptions;
        }

        public Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        public Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }


        public virtual bool ShouldExecute(ExecutionContext<T> context)
        {
            return true;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ShouldExecuteFunc(context));
        }

        protected virtual NodeResultStatus PerformExecute(ExecutionContext<T> context)
        {
            return NodeResultStatus.Succeeded;
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ExecutedFunc(context));
        }
    }
}