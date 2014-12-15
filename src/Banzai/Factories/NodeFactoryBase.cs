using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Banzai.Utility;

namespace Banzai.Factories
{

    /// <summary>
    /// Base class for untyped node factories
    /// </summary>
    public abstract class NodeFactoryBase : INodeFactory
    {
        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract TNode GetNode<TNode>();

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract TNode GetNode<TNode>(string name);

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public abstract IEnumerable<TNode> GetAllNodes<TNode>();

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        public INode<T> GetFlow<T>(string name)
        {
            Guard.AgainstNullOrEmptyArgument("name", name);

            var flowRoot = GetFlowRoot<T>(name);

            return BuildNode(flowRoot.Children[0], flowRoot.ShouldExecuteFuncAsync, flowRoot.ShouldExecuteFunc);
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract INode<T> GetNode<T>(Type type);

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract INode<T> GetNode<T>(Type type, string name);

        /// <summary>
        /// Method overridden to provide a root FlowComponent based on a name.
        /// </summary>
        /// <param name="name">Name of the flow root.</param>
        /// <returns>FlowComponent corresponding to the named root.</returns>
        protected abstract FlowComponent<T> GetFlowRoot<T>(string name);

        /// <summary>
        /// Applies metadata to the node during node construction.
        /// </summary>
        /// <typeparam name="T">Type of node subject.</typeparam>
        /// <param name="node">Node to which metadata is applied.</param>
        /// <param name="metaData">MetaData to apply.</param>
        protected abstract void ApplyMetaData<T>(INode<T> node, IDictionary<string, object> metaData);

        /// <summary>
        /// Builds a node from the provided FlowComponent.
        /// </summary>
        /// <param name="component">Flowcomponent providing the node definition.</param>
        /// <param name="shouldExecuteFunc">Allows a ShouldExecuteFunc to be specified from the parent.</param>
        /// <param name="shouldExecuteFuncAsync">Allows a ShouldExecuteAsyncFunc to be specified from the parent.</param>
        /// <returns>A constructed INode.</returns>
        protected INode<T> BuildNode<T>(FlowComponent<T> component,
            Func<IExecutionContext<T>, Task<bool>> shouldExecuteFuncAsync = null, Func<IExecutionContext<T>, bool> shouldExecuteFunc = null)
        {
            INode<T> node;
            //Get the node or flow from the flowComponent
            if (component.IsFlow)
            {
                node = GetFlow<T>(component.Name);
            }
            else
            {
                node = string.IsNullOrEmpty(component.Name) ? GetNode<T>(component.Type) : GetNode<T>(component.Type, component.Name);
            }

            if (component.ShouldExecuteFuncAsync != null)
            {
                node.AddShouldExecute(component.ShouldExecuteFuncAsync);
            }
            else if (shouldExecuteFuncAsync != null)
            {
                node.AddShouldExecute(shouldExecuteFuncAsync);
            }
            if (component.ShouldExecuteFunc != null)
            {
                node.AddShouldExecute(component.ShouldExecuteFunc);
            }
            else if (shouldExecuteFunc != null)
            {
                node.AddShouldExecute(shouldExecuteFunc);
            }

            if (component.MetaData != null && component.MetaData.Count > 0)
            {
                ApplyMetaData(node, component.MetaData);
            }

            if (component.Children != null && component.Children.Count > 0)
            {
                var multiNode = (IMultiNode<T>)node;
                foreach (var childComponent in component.Children)
                {
                    multiNode.AddChild(BuildNode(childComponent));
                }
            }

            return node;
        }

    }

    /// <summary>
    /// Base class for node factories.
    /// </summary>
    /// <typeparam name="T">Type of the subject of the flow.</typeparam>
    public abstract class NodeFactoryBase<T> : INodeFactory<T>
    {
        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract TNode GetNode<TNode>() where TNode : INode<T>;

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract TNode GetNode<TNode>(string name) where TNode : INode<T>;

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract INode<T> GetNode(Type type);

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public abstract INode<T> GetNode(Type type, string name);

        /// <summary>
        /// Gets the default registered node for the specified types.
        /// </summary>
        /// <param name="types">Types of the nodes to return.</param>
        /// <returns>The first node matching each TNode type.</returns>
        public abstract IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types);

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public abstract IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>;

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        public INode<T> GetFlow(string name)
        {
            Guard.AgainstNullOrEmptyArgument("name", name);

            var flowRoot = GetFlowRoot(name);

            return BuildNode(flowRoot.Children[0], flowRoot.ShouldExecuteFuncAsync, flowRoot.ShouldExecuteFunc);
        }

        /// <summary>
        /// Method overridden to provide a root FlowComponent based on a name.
        /// </summary>
        /// <param name="name">Name of the flow root.</param>
        /// <returns>FlowComponent corresponding to the named root.</returns>
        protected abstract FlowComponent<T> GetFlowRoot(string name);

        /// <summary>
        /// Applies metadata to the node during node construction.
        /// </summary>
        /// <param name="node">Node to which metadata is applied.</param>
        /// <param name="metaData">Metadata to apply to the node.</param>
        protected abstract void ApplyMetaData(INode<T> node, IDictionary<string, object> metaData);

        /// <summary>
        /// Builds a node from the provided FlowComponent.
        /// </summary>
        /// <param name="component">Flowcomponent providing the node definition.</param>
        /// <param name="shouldExecuteFunc">Allows a ShouldExecuteFunc to be specified from the parent.</param>
        /// <param name="shouldExecuteFuncAsync">Allows a ShouldExecuteAsyncFunc to be specified from the parent.</param>
        /// <returns>A constructed INode.</returns>
        protected INode<T> BuildNode(FlowComponent<T> component,
            Func<IExecutionContext<T>, Task<bool>> shouldExecuteFuncAsync = null, Func<IExecutionContext<T>, bool> shouldExecuteFunc = null)
        {
            INode<T> node;
            //Get the node or flow from the flowComponent
            if (component.IsFlow)
            {
                node = GetFlow(component.Name);
            }
            else
            {
                node = string.IsNullOrEmpty(component.Name) ? GetNode(component.Type) : GetNode(component.Type, component.Name);
            }

            if (component.ShouldExecuteFuncAsync != null)
            {
                node.AddShouldExecute(component.ShouldExecuteFuncAsync);
            }
            else if (shouldExecuteFuncAsync != null)
            {
                node.AddShouldExecute(shouldExecuteFuncAsync);
            }
            if (component.ShouldExecuteFunc != null)
            {
                node.AddShouldExecute(component.ShouldExecuteFunc);
            }
            else if (shouldExecuteFunc != null)
            {
                node.AddShouldExecute(shouldExecuteFunc);
            }

            if (component.MetaData != null && component.MetaData.Count > 0)
            {
                ApplyMetaData(node, component.MetaData);
            }

            if (component.Children != null && component.Children.Count > 0)
            {
                var multiNode = (IMultiNode<T>)node;
                foreach (var childComponent in component.Children)
                {
                    multiNode.AddChild(BuildNode(childComponent));
                }
            }

            return node;
        }

    }

}