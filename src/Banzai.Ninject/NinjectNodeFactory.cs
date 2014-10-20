using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Banzai.Factories;
using Ninject.Syntax;

namespace Banzai.Ninject
{
    /// <summary>
    /// Untyped NodeFactory using Ninject as the underlying provider.
    /// </summary>
    public class NinjectNodeFactory : NodeFactoryBase
    {
        private readonly IResolutionRoot _resolver;

        /// <summary>
        /// Constructs a new NinjectNodeFactory
        /// </summary>
        /// <param name="resolver">IResolutionRoot used to provide instances.</param>
        public NinjectNodeFactory(IResolutionRoot resolver)
        {
            _resolver = resolver;
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode<T>(Type type)
        {
            return (INode<T>)_resolver.Get(type);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>(string name)
        {
            return _resolver.Get<TNode>(name);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode<T>(Type type, string name)
        {
            return (INode<T>)_resolver.Get(type, name);
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>()
        {
            return _resolver.Get<TNode>();
        }

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public override IEnumerable<TNode> GetAllNodes<TNode>()
        {
            return _resolver.GetAll<TNode>();
        }

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        protected override FlowComponent<T> GetFlowRoot<T>(string name)
        {
            return _resolver.Get<FlowComponent<T>>(name);
        }

    }


    /// <summary>
    /// NodeFactory using Ninject as the underlying provider.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public class NinjectNodeFactory<T> : NodeFactoryBase<T>
    {
        private readonly IResolutionRoot _resolver;

        /// <summary>
        /// Constructs a new NinjectNodeFactory
        /// </summary>
        /// <param name="resolver">IResolutionRoot used to provide instances.</param>
        public NinjectNodeFactory(IResolutionRoot resolver)
        {
            _resolver = resolver;
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>() 
        {
            return _resolver.Get<TNode>();
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <typeparam name="TNode">Type of the node to return.</typeparam>
        /// <returns>The first node matching the TNode type.</returns>
        public override TNode GetNode<TNode>(string name)
        {
            return _resolver.Get<TNode>(name);
        }

        /// <summary>
        /// Gets a node by the specified type.
        /// </summary>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode(Type type)
        {
            return (INode<T>)_resolver.Get(type);
        }

        /// <summary>
        /// Gets a node by the specified type and registered name.
        /// </summary>
        /// <param name="name">Name of the node to return (as registered).</param>
        /// <param name="type">Type of the node to return.</param>
        /// <returns>The first node matching the TNode type.</returns>
        public override INode<T> GetNode(Type type, string name)
        {
            return (INode<T>)_resolver.Get(type, name);
        }

        /// <summary>
        /// Gets the default registered node for the specified types.
        /// </summary>
        /// <param name="types">Types of the nodes to return.</param>
        /// <returns>The first node matching each TNode type.</returns>
        public override IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types)
        {
            return types.Select(type => (INode<T>) _resolver.Get(type));
        }

        /// <summary>
        /// Gets all nodes matching the requested type.
        /// </summary>
        /// <typeparam name="TNode">Type of the nodes to return.</typeparam>
        /// <returns>Enumerable of nodes matching the requested type.</returns>
        public override IEnumerable<TNode> GetAllNodes<TNode>() 
        {
            return _resolver.Get<IEnumerable<TNode>>();
        }

        /// <summary>
        /// Gets a flow matching the specified name and subject type.
        /// </summary>
        /// <param name="name">Name of flow to return.</param>
        /// <returns>Flow matching the requested criteria.</returns>
        protected override FlowComponent<T> GetFlowRoot(string name)
        {
            return _resolver.Get<FlowComponent<T>>(name);
        }
    }
}