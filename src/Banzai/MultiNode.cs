using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banzai.Core
{
    public interface IMultiNode<T> : INode<T>
    {
        IReadOnlyList<INode<T>> Children { get; }

        void AddChild(INode<T> child);

        void AddChildren(IEnumerable<INode<T>> children);

        void RemoveChild(INode<T> child);
    }


    public abstract class MultiNode<T> : Node<T>, IMultiNode<T>
    {
        private readonly List<INode<T>> _children = new List<INode<T>>();

        public IReadOnlyList<INode<T>> Children { get { return _children; } }

        public void AddChild(INode<T> child)
        {
            _children.Add(child);
        }

        public void AddChildren(IEnumerable<INode<T>> children)
        {
            _children.AddRange(children);
        }

        public void RemoveChild(INode<T> child)
        {
            _children.Remove(child);
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

        protected override async Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            if (Children == null || Children.Count == 0)
            {
                return NodeResultStatus.NotRun;
            }

            return await ExecuteChildrenAsync(context);
        }

        protected override ExecutionContext<T> PrepareExecutionContext(ExecutionContext<T> context, NodeResult<T> currentResult)
        {
            var derivedContext = new ExecutionContext<T>(context, currentResult);
            derivedContext.AddResult(currentResult);

            if (LocalOptions != null)
                derivedContext.EffectiveOptions = LocalOptions;

            return derivedContext;
        }

        protected abstract Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context);

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