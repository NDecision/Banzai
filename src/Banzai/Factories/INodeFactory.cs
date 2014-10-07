using System;
using System.Collections.Generic;

namespace Banzai.Factories
{

    /// <summary>
    /// Interface for the node factory.  Used to create child nodes.
    /// </summary>
    /// <typeparam name="T">Type of the underlying node subject.</typeparam>
    public interface INodeFactory<T> 
    {
        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        TNode GetNode<TNode>() where TNode : INode<T>;

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        TNode GetNode<TNode>(string name) where TNode : INode<T>;

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        INode<T> GetNode(Type type);

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        INode<T> GetNode(Type type, string name);

        /// <summary>
        /// Gets the default registered node for the specified types.
        /// </summary>
        /// <param name="types">Types of the nodes to return.</param>
        /// <returns>The first node matching each TNode type.</returns>
        IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types);

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>;

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        INode<T> GetFlow(string name);

    }

}