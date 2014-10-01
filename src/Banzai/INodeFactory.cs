using System;
using System.Collections.Generic;

namespace Banzai
{
    public interface INodeFactory<T>
    {
        TNode GetNode<TNode>() where TNode : INode<T>;

        TNode GetNode<TNode>(string name) where TNode : INode<T>; 

        INode<T> GetNode(Type type);

        INode<T> GetNode(Type type, string name);

        IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types);

        IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>;
    }

}