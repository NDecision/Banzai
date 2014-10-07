using System;
using System.Collections.Generic;
using System.Linq;
using Banzai.Utility;

namespace Banzai.Factories
{

    /// <summary>
    /// Allows the construction of a root flow.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public interface IFlowBuilder<T>
    {
        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        IFlowComponentBuilder<T> AddRoot<TNode>(string name = null) where TNode : INode<T>;

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        IFlowComponentBuilder<T> AddRoot(Type nodeType, string name = null);
    }

    /// <summary>
    /// Allows the addition of flow components (nodes or subflows) to a parent flow or component.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public interface IFlowComponentBuilder<T>
    {
        /// <summary>
        /// Adds a previously registered flow by name as a child of this node.
        /// </summary>
        /// <param name="name">The name of the flow to add.</param>
        /// <returns>The current FlowComponent instance.</returns>
        IFlowComponentBuilder<T> AddFlow(string name);

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        IFlowComponentBuilder<T> AddChild<TNode>(string name = null) where TNode : INode<T>;

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        IFlowComponentBuilder<T> AddChild(Type nodeType, string name = null);

        /// <summary>
        /// Returns an instance of FlowComponent representing the requested child node.
        /// </summary>
        /// <typeparam name="TNode">Type of the node.</typeparam>
        /// <param name="name">Optional name of the node in IOC registration.</param>
        /// <param name="index">Index of the node if multiple matches are found in the parent.  Defaults to first.</param>
        /// <returns>A builder for the located child FlowComponent of this FlowComponent.</returns>
        IFlowComponentBuilder<T> ForChild<TNode>(string name = null, int index = 0) where TNode : INode<T>;

        /// <summary>
        /// Returns an instance of FlowComponent representing the requested child node.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="name">Optional name of the node in IOC registration.</param>
        /// <param name="index">Index of the node if multiple matches are found in the parent.  Defaults to first.</param>
        /// <returns>A builder for the located child FlowComponent of this FlowComponent.</returns>
        IFlowComponentBuilder<T> ForChild(Type nodeType, string name = null, int index = 0);
    }


    public class FlowComponentBuilder<T> : IFlowComponentBuilder<T>, IFlowBuilder<T>
    {
        private readonly FlowComponent<T> _component;

        public FlowComponentBuilder(FlowComponent<T> component)
        {
            _component = component;
        }

        /// <summary>
        /// Adds a previously registered flow by name as a child of this node.
        /// </summary>
        /// <param name="name">The name of the flow to add.</param>
        /// <returns>The current FlowComponent instance.</returns>
        public IFlowComponentBuilder<T> AddFlow(string name)
        {
            Guard.AgainstNullOrEmptyArgument("name", name);

            if (!typeof(IMultiNode<T>).IsAssignableFrom(_component.Type))
                throw new InvalidOperationException("In order to have children, nodeType must be assignable to IMultiNode<T>.");

            _component.AddChild(new FlowComponent<T> { Type = typeof(T), Name = name, IsFlow = true });
            return this;
        }

        /// <summary>
        /// Adds a root node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        public IFlowComponentBuilder<T> AddRoot<TNode>(string name = null) where TNode : INode<T>
        {
            return AddRoot(typeof(TNode), name);
        }

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        public IFlowComponentBuilder<T> AddRoot(Type nodeType, string name = null)
        {
            if (!_component.IsFlow)
                throw new InvalidOperationException("This method is only valid for flow components.");

            if (!typeof(INode<T>).IsAssignableFrom(nodeType))
                throw new ArgumentException("nodeType must be assignable to INode<T>.", "nodeType");

            var child = _component.AddChild(new FlowComponent<T> { Type = nodeType, Name = name });
            return new FlowComponentBuilder<T>(child);
        }

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        public IFlowComponentBuilder<T> AddChild<TNode>(string name = null) where TNode : INode<T>
        {
            return AddChild(typeof(TNode), name);
        }

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <returns>The current FlowComponent instance.</returns>
        public IFlowComponentBuilder<T> AddChild(Type nodeType, string name = null)
        {
            if (!typeof(INode<T>).IsAssignableFrom(nodeType))
                throw new ArgumentException("nodeType must be assignable to INode<T>.", "nodeType");

            if (!typeof(IMultiNode<T>).IsAssignableFrom(_component.Type))
                throw new InvalidOperationException("In order to have children, nodeType must be assignable to IMultiNode<T>.");

            _component.AddChild(new FlowComponent<T> { Type = nodeType, Name = name });
            return this;
        }

        /// <summary>
        /// Returns an instance of FlowComponent representing the requested child node.
        /// </summary>
        /// <typeparam name="TNode">Type of the node.</typeparam>
        /// <param name="name">Optional name of the node in IOC registration.</param>
        /// <param name="index">Index of the node if multiple matches are found in the parent.  Defaults to first.</param>
        /// <returns>A child FlowComponent of this FlowComponent.</returns>
        public IFlowComponentBuilder<T> ForChild<TNode>(string name = null, int index = 0) where TNode : INode<T>
        {
            return ForChild(typeof(TNode), name, index);
        }

        /// <summary>
        /// Returns an instance of FlowComponent representing the requested child node.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="name">Optional name of the node in IOC registration.</param>
        /// <param name="index">Index of the node if multiple matches are found in the parent.  Defaults to first.</param>
        /// <returns>A child FlowComponent of this FlowComponent.</returns>
        public IFlowComponentBuilder<T> ForChild(Type nodeType, string name = null, int index = 0)
        {
            if (!typeof(INode<T>).IsAssignableFrom(nodeType))
                throw new ArgumentException("nodeType must be assignable to INode<T>.", "nodeType");

            var items = _component.Children.Where(x => x.Type == nodeType);
            if (name != null)
                items = items.Where(x => x.Name == name);

            IList<FlowComponent<T>> results = items.ToList();

            if (index + 1 > results.Count)
                throw new IndexOutOfRangeException("The requested child could not be found.");

            var child = results[index];

            return new FlowComponentBuilder<T>(child);
        }
    }
}