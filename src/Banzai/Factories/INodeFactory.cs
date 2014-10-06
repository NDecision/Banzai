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
        TNode GetNode<TNode>() where TNode : INode<T>;

        TNode GetNode<TNode>(string name) where TNode : INode<T>; 

        INode<T> GetNode(Type type);

        INode<T> GetNode(Type type, string name);

        IEnumerable<INode<T>> GetNodes(IEnumerable<Type> types);

        IEnumerable<TNode> GetAllNodes<TNode>() where TNode : INode<T>;

        INode<T> GetFlow(string name);

    }

}