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
            _children.Add(child);
        }

        /// <summary>
        /// Adds multiple child nodes to this node.
        /// </summary>
        /// <param name="children">Children to add.</param>
        public void AddChildren(IEnumerable<INode<T>> children)
        {
            _children.AddRange(children);
        }

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove.</param>
        public void RemoveChild(INode<T> child)
        {
            _children.Remove(child);
        }

        /// <summary>
        /// Resets this node and all its children to an unrun state.
        /// </summary>
        public override void Reset()
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
        protected override async Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            if (Children == null || Children.Count == 0)
            {
                return NodeResultStatus.NotRun;
            }

            return await ExecuteChildrenAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="currentResult">A referene to the result of the current node.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected override ExecutionContext<T> PrepareExecutionContext(ExecutionContext<T> context, NodeResult<T> currentResult)
        {
            var derivedContext = new ExecutionContext<T>(context, currentResult);

            context.AddResult(currentResult);

            if (LocalOptions != null)
                derivedContext.EffectiveOptions = LocalOptions;

            return derivedContext;
        }

        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected abstract Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context);

        /// <summary>
        /// Aggregates the child results of a MultiNode into a summary result status.
        /// </summary>
        /// <param name="results">Results to aggregate.</param>
        /// <param name="options">Execution options to consider during aggregation.</param>
        /// <returns>Summary NodeResultStatus.</returns>
        protected static NodeResultStatus AggregateNodeResults(IEnumerable<NodeResult<T>> results, ExecutionOptions options)
        {
            bool hasFailure = false;
            bool hasSuccess = false;
            bool hasSuccessWithErrors = false;

            foreach (var nodeResult in results)
            {
                if (!hasSuccessWithErrors && nodeResult.Status == NodeResultStatus.SucceededWithErrors)
                {
                    hasSuccessWithErrors = true;
                }
                else if (!hasFailure && nodeResult.Status == NodeResultStatus.Failed)
                {
                    hasFailure = true;
                }
                else if (!hasSuccess && nodeResult.Status == NodeResultStatus.Succeeded)
                {
                    hasSuccess = true;
                }
                if (hasSuccess && hasFailure)
                {
                    hasSuccessWithErrors = true;
                    break;
                }
            }

            if (hasSuccessWithErrors)
            {
                if (hasFailure && !options.ContinueOnFailure)
                {
                    return NodeResultStatus.Failed;
                }

                return NodeResultStatus.SucceededWithErrors;
            }
            
            if (hasSuccess)
                return NodeResultStatus.Succeeded;
            if (hasFailure)
                return NodeResultStatus.Failed;

            return NodeResultStatus.NotRun;
        }
    }
}