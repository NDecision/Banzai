using System.Collections.Generic;

namespace Banzai.Core
{
    public abstract class GroupNode<T> : Node<T>, IGroupNode<T>
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

        protected static NodeResultStatus AggregateNodeResults(IEnumerable<NodeResult<T>> results)
        {
            bool hasFailure = false;
            bool hasSuccess = false;
            bool hasSuccessWithErrors = false;

            foreach (var nodeResult in results)
            {
                if (nodeResult.Status == NodeResultStatus.SucceededWithErrors)
                {
                    hasSuccessWithErrors = true;
                    break;
                }

                if (nodeResult.Status == NodeResultStatus.Failed)
                {
                    hasFailure = true;
                }
                else if (nodeResult.Status == NodeResultStatus.Succeeded)
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
                return NodeResultStatus.SucceededWithErrors;
            if (hasSuccess)
                return NodeResultStatus.Succeeded;
            if (hasFailure)
                return NodeResultStatus.Failed;

            return NodeResultStatus.NotRun;
        }
    }
}