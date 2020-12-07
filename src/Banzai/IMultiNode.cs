using System.Collections.Generic;
using Banzai.Factories;

namespace Banzai
{
    /// <summary>
    ///     Basis for other multinodes (Pipleline/GroupNode/FirstMatch)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultiNode<T> : INode<T>
    {
        /// <summary>
        ///     Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        INodeFactory<T> NodeFactory { get; set; }

        /// <summary>
        ///     Gets the children of this node.
        /// </summary>
        IReadOnlyList<INode<T>> Children { get; }

        /// <summary>
        ///     Adds a child node to this node.
        /// </summary>
        /// <param name="child">Child node to add.</param>
        void AddChild(INode<T> child);

        /// <summary>
        ///     Adds multiple child nodes to this node.
        /// </summary>
        /// <param name="children">Children to add.</param>
        void AddChildren(IEnumerable<INode<T>> children);

        /// <summary>
        ///     Removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove.</param>
        void RemoveChild(INode<T> child);
    }
}