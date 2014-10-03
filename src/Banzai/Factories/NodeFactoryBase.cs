using System;
using System.Collections.Generic;
using Banzai.Utility;

namespace Banzai.Factories
{
    public abstract class NodeFactoryBase<T> : INodeFactory<T>
    {
        public abstract TNode GetNode<TNode>() where TNode : INode<T>;
        public abstract TNode GetNode<TNode>(string name) where TNode : INode<T>;
        public abstract INode<T> GetNode(Type type);
        public abstract INode<T> GetNode(Type type, string name);
        public abstract IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types);
        public abstract IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>;

        public INode<T> GetFlow(string name)
        {
            Guard.AgainstNullOrEmptyArgument("name", name);

            var flowRoot = GetFlowRoot(name);

            return BuildNode(flowRoot.Children[0]);
        }


        public INode<T> BuildNode(FlowComponent<T> component)
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

            if (component.Children != null && component.Children.Count > 0)
            {
                var multiNode = (IMultiNode<T>) node;
                foreach (var childComponent in component.Children)
                {
                    multiNode.AddChild(BuildNode(childComponent));
                }
            }

            return node;
        }

        protected abstract FlowComponent<T> GetFlowRoot(string name);


    }
}