using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Banzai.Factories;

namespace Banzai.Autofac
{
    /// <summary>
    /// Untyped NodeFactory using Autofac as the underlying provider.
    /// </summary>
    public class AutofacNodeFactory : NodeFactoryBase
    {
        private readonly IComponentContext _componentContext;

        /// <summary>
        /// Constructs a new AutofacNodeFactory
        /// </summary>
        /// <param name="componentContext">IComponentContext used to provide instances.</param>
        public AutofacNodeFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode<T>(Type type)
        {
            return (INode<T>)_componentContext.Resolve(type);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>(string name)
        {
            return _componentContext.ResolveNamed<TNode>(name);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode<T>(Type type, string name)
        {
            return (INode<T>)_componentContext.ResolveNamed(name, type);
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>()
        {
            return _componentContext.Resolve<TNode>();
        }

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public override IEnumerable<TNode> GetAllNodes<TNode>()
        {
            return _componentContext.Resolve<IEnumerable<TNode>>();
        }

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        protected override FlowComponent<T> GetFlowRoot<T>(string name)
        {
            return _componentContext.ResolveNamed<FlowComponent<T>>(name);
        }

        /// <summary>
        /// Applies metadata to the node during node construction.
        /// </summary>
        /// <param name="node">Node to which metadata is applied.</param>
        /// <param name="metaData">MetaData to apply.</param>
        protected override void ApplyMetaData<T>(INode<T> node, IDictionary<string, object> metaData)
        {
            var builders = _componentContext.Resolve<IEnumerable<IMetaDataBuilder>>();

            if (builders != null)
            {
                foreach (var builder in builders)
                {
                    builder.Apply(node, metaData);
                }
            }
        }

    }


    /// <summary>
    /// NodeFactory using Autofac as the underlying provider.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public class AutofacNodeFactory<T> : NodeFactoryBase<T>
    {
        private readonly IComponentContext _componentContext;

        /// <summary>
        /// Constructs a new AutofacNodeFactory
        /// </summary>
        /// <param name="componentContext">IComponentContext used to provide instances.</param>
        public AutofacNodeFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>() 
        {
            return _componentContext.Resolve<TNode>();
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>(string name)
        {
            return _componentContext.ResolveNamed<TNode>(name);
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode(Type type)
        {
            return (INode<T>)_componentContext.Resolve(type);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode(Type type, string name)
        {
            return (INode<T>)_componentContext.ResolveNamed(name, type);
        }

        /// <summary>
        /// Gets the default registered node for the specified types.
        /// </summary>
        /// <param name="types">Types of the nodes to return.</param>
        /// <returns>The first node matching each TNode type.</returns>
        public override IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types)
        {
            return types.Select(type => (INode<T>) _componentContext.Resolve(type));
        }

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public override IEnumerable<TNode> GetAllNodes<TNode>() 
        {
            return _componentContext.Resolve<IEnumerable<TNode>>();
        }

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        protected override FlowComponent<T> GetFlowRoot(string name)
        {
            return _componentContext.ResolveNamed<FlowComponent<T>>(name);
        }

        /// <summary>
        /// Applies metadata to the node during node construction.
        /// </summary>
        /// <param name="node">Node to which metadata is applied.</param>
        /// <param name="metaData">MetaData to apply</param>
        protected override void ApplyMetaData(INode<T> node, IDictionary<string, object> metaData)
        {
            var builders = _componentContext.Resolve<IEnumerable<IMetaDataBuilder>>();

            if (builders != null)
            {
                foreach (var builder in builders)
                {
                    builder.Apply(node, metaData);
                }
            }
        }
    }
}