using System.Collections.Generic;
using System.Threading.Tasks;
using Banzai.Factories;

namespace Banzai
{

    /// <summary>
    /// Basis for other multinodes (Pipleline/GroupNode/FirstMatch)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultiNode<T> : INode<T>
    {
        /// <summary>
        /// Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        INodeFactory<T> NodeFactory { get; set; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        IReadOnlyList<INode<T>> Children { get; }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="child">Child node to add.</param>
        void AddChild(INode<T> child);

        /// <summary>
        /// Adds multiple child nodes to this node.
        /// </summary>
        /// <param name="children">Children to add.</param>
        void AddChildren(IEnumerable<INode<T>> children);

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove.</param>
        void RemoveChild(INode<T> child);

    }

    /// <summary>
    /// Base class to use for other MultiNodes(Pipeline/Group/FirstMatch)
    /// </summary>
    /// <typeparam name="T">Type of the subject that the node operates on.</typeparam>
    public abstract class MultiNode<T> : Node<T>, IMultiNode<T>
    {
        private readonly List<INode<T>> _children = new List<INode<T>>();

        /// <summary>
        /// Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        public INodeFactory<T> NodeFactory { get; set; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public IReadOnlyList<INode<T>> Children { get { return _children; } }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="child">Child node to add.</param>
        public void AddChild(INode<T> child)
        {
            LogWriter.Debug("Added child node.");
            _children.Add(child);
        }

        /// <summary>
        /// Adds multiple child nodes to this node.
        /// </summary>
        /// <param name="children">Children to add.</param>
        public void AddChildren(IEnumerable<INode<T>> children)
        {
            LogWriter.Debug("Added children.");
            _children.AddRange(children);
        }

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove.</param>
        public void RemoveChild(INode<T> child)
        {
            LogWriter.Debug("Removed child node.");
            _children.Remove(child);
        }

        /// <summary>
        /// Resets this node and all its children to an unrun state.
        /// </summary>
        public override sealed void Reset()
        {
            base.Reset();
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected override sealed async Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            if (Children == null || Children.Count == 0)
            {
                return NodeResultStatus.NotRun;
            }

            LogWriter.Debug("Preparing to run child nodes.");
            NodeResultStatus status = await ExecuteChildrenAsync(context).ConfigureAwait(false);
            LogWriter.Debug("Completed running child nodes.");
            return status;
        }

        /// <summary>
        /// Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="result">The result reference to add to the current context.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected override sealed IExecutionContext<T> PrepareExecutionContext(IExecutionContext<T> context, NodeResult result)
        {
            LogWriter.Debug("Preparing execution context.");
            var resultContext = new ExecutionContext<T>(context, result);

            context.AddResult(result);

            return resultContext;
        }

        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected abstract Task<NodeResultStatus> ExecuteChildrenAsync(IExecutionContext<T> context);

    }
}