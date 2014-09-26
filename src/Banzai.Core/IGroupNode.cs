using System.Collections.Generic;

namespace Banzai.Core
{
    public interface IGroupNode<T> : INode<T>
    {
        IReadOnlyList<INode<T>> Children { get; }

        void AddChild(INode<T> child);

        void AddChildren(IEnumerable<INode<T>> children);

        void RemoveChild(INode<T> child); 
    }
}