using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Banzai.Autofac
{
    public class AutofacNodeFactory<T> : INodeFactory<T>
    {
        private readonly IComponentContext _componentContext;

        public AutofacNodeFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public TNode GetNode<TNode>() where TNode : INode<T>
        {
            return _componentContext.Resolve<TNode>();
        }

        public TNode GetNode<TNode>(string name) where TNode : INode<T>
        {
            return _componentContext.ResolveNamed<TNode>(name);
        }

        public INode<T> GetNode(Type type)
        {
            return (INode<T>)_componentContext.Resolve(type);
        }

        public INode<T> GetNode(Type type, string name)
        {
            return (INode<T>)_componentContext.ResolveNamed(name, type);
        }

        public IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types)
        {
            return types.Select(type => (INode<T>) _componentContext.Resolve(type));
        }

        public IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>
        {
            return _componentContext.Resolve<IEnumerable<TNode>>();
        }
    }
}