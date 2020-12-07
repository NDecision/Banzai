using Banzai.Factories;

namespace Banzai
{
    /// <summary>
    /// Interface for a Node that allows a transition to another node type.
    /// </summary>
    /// <typeparam name="TSource">Source node type.</typeparam>
    /// <typeparam name="TDestination">Destination node type.</typeparam>
    public interface ITransitionNode<in TSource, TDestination> : INode<TSource>
    {
        /// <summary>
        /// Gets or sets the destionation child node to execute.
        /// </summary>
        INode<TDestination> ChildNode { get; set; }

        /// <summary>
        /// Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        INodeFactory<TDestination> NodeFactory { get; set; }
    }
}