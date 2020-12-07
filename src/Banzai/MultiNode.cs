using System.Collections.Generic;
using System.Threading.Tasks;
using Banzai.Factories;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    ///     Base class to use for other MultiNodes(Pipeline/Group/FirstMatch)
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public abstract class MultiNode<T> : Node<T>, IMultiNode<T>
    {
        private readonly List<INode<T>> _children = new List<INode<T>>();

        /// <summary>
        ///     Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        public INodeFactory<T> NodeFactory { get; set; }

        /// <summary>
        ///     Gets the children of this node.
        /// </summary>
        public IReadOnlyList<INode<T>> Children => _children;

        /// <summary>
        ///     Adds a child node to this node.
        /// </summary>
        /// <param name="child">Child node to add.</param>
        public void AddChild(INode<T> child)
        {
            Logger.LogDebug("Added child node.");
            _children.Add(child);
        }

        /// <summary>
        ///     Adds multiple child nodes to this node.
        /// </summary>
        /// <param name="children">Children to add.</param>
        public void AddChildren(IEnumerable<INode<T>> children)
        {
            Logger.LogDebug("Added children.");
            _children.AddRange(children);
        }

        /// <summary>
        ///     Removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove.</param>
        public void RemoveChild(INode<T> child)
        {
            Logger.LogDebug("Removed child node.");
            _children.Remove(child);
        }

        /// <summary>
        ///     Resets this node and all its children to an unrun state.
        /// </summary>
        public sealed override void Reset()
        {
            base.Reset();
            foreach (var child in Children) child.Reset();
        }

        /// <summary>
        ///     Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected sealed override async Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            if (Children == null || Children.Count == 0) return NodeResultStatus.NotRun;

            Logger.LogDebug("Preparing to run child nodes.");
            var status = await ExecuteChildrenAsync(context).ConfigureAwait(false);
            Logger.LogDebug("Completed running child nodes.");
            return status;
        }

        /// <summary>
        ///     Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="result">The result reference to add to the current context.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected sealed override IExecutionContext<T> PrepareExecutionContext(IExecutionContext<T> context,
            NodeResult result)
        {
            Logger.LogDebug("Preparing execution context.");
            var resultContext = new ExecutionContext<T>(context, result);

            context.AddResult(result);

            return resultContext;
        }

        /// <summary>
        ///     Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected abstract Task<NodeResultStatus> ExecuteChildrenAsync(IExecutionContext<T> context);
    }
}