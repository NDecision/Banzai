using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Banzai.Factories;

namespace Banzai.Autofac
{
    public class AutofacNodeFactory<T> : NodeFactoryBase<T>
    {
        private readonly IComponentContext _componentContext;

        public AutofacNodeFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public override TNode GetNode<TNode>() 
        {
            return _componentContext.Resolve<TNode>();
        }

        public override TNode GetNode<TNode>(string name)
        {
            return _componentContext.ResolveNamed<TNode>(name);
        }

        public override INode<T> GetNode(Type type)
        {
            return (INode<T>)_componentContext.Resolve(type);
        }

        public override INode<T> GetNode(Type type, string name)
        {
            return (INode<T>)_componentContext.ResolveNamed(name, type);
        }

        public override IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types)
        {
            return types.Select(type => (INode<T>) _componentContext.Resolve(type));
        }

        public override IEnumerable<TNode> GetAllNodes<TNode>() 
        {
            return _componentContext.Resolve<IEnumerable<TNode>>();
        }


        protected override FlowComponent<T> GetFlowRoot(string name)
        {
            return _componentContext.ResolveNamed<FlowComponent<T>>(name);
        }
    }
}