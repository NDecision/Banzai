using System;

namespace Banzai.Factories
{
    /// <summary>
    /// Allows the construction of a root flow.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public interface IFlowBuilder<out T>
    {
        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <param name="id">Id of the flow.  This can be used for identification in debugging.  Flows default to the flow name.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddRoot<TNode>(string name = null, string id = null) where TNode : INode<T>;

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <param name="id">Id of the flow.  This can be used for identification in debugging.  Flows default to the flow name.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddRoot(Type nodeType, string name = null, string id = null);
    }
}