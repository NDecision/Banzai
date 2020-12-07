using System;
using System.Threading.Tasks;

namespace Banzai.Factories
{
    /// <summary>
    /// Allows the addition of flow components (nodes or subflows) to a parent flow or component.
    /// </summary>
    /// <typeparam name="T">Type of the flow subject.</typeparam>
    public interface IFlowComponentBuilder<out T>
    {
        /// <summary>
        /// Adds a previously registered flow by name as a child of this node.
        /// </summary>
        /// <param name="name">The name of the flow to add.</param>
        /// <param name="id">Id of the flow. This can be used for identification in debugging. Defaults to the flow name.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddFlow(string name, string id = null);

        /// <summary>
        /// Adds a previously registered flow by name as a child of this node.
        /// </summary>
        /// <param name="name">The name of the flow to add.</param>
        /// <param name="id">Id of the flow. This can be used for identification in debugging. Defaults to the flow name.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddFlow<TNode>(string name, string id = null);

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <typeparam name="TNode">Type of the node to add.</typeparam>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <param name="id">Id of the node. This can be used for identification in debugging. Defaults to the node type with the name if included.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddChild<TNode>(string name = null, string id = null) where TNode : INode<T>;

        /// <summary>
        /// Adds a child node to this flow.
        /// </summary>
        /// <param name="nodeType">Type of the node to add.</param>
        /// <param name="name">Optional name of the node if needed to find in IOC container.</param>
        /// <param name="id">Id of the node. This can be used for identification in debugging. Defaults to the node type with the name if included.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> AddChild(Type nodeType, string name = null, string id = null);

        /// <summary>
        /// Adds a ShouldExecuteBlock to the flowcomponent (to be added to the resultant node).
        /// </summary>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> SetShouldExecuteBlock<TBlock>() where TBlock : IShouldExecuteBlock<T>;

        /// <summary>
        /// Adds a ShouldExecuteBlock to the flowcomponent (to be added to the resultant node).
        /// </summary>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> SetShouldExecuteBlock(Type blockType);

        /// <summary>
        /// Adds a ShouldExecuteAsync to the flowcomponent (to be added to the resultant node).
        /// </summary>
        /// <param name="shouldExecuteFunc">Function to add as ShouldExecute to the flowcomponent.</param>
        /// <returns>The current FlowComponentBuilder instance.</returns>
        IFlowComponentBuilder<T> SetShouldExecute(Func<IExecutionContext<T>, Task<bool>> shouldExecuteFunc);

        /// <summary>
        /// Allows metadata about the flow component to be added.
        /// </summary>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <returns></returns>
        IFlowComponentBuilder<T> SetMetaData(string key, object data);
            
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

        /// <summary>
        /// Returns an instance of FlowComponentBuilder representing the requested child flow.
        /// </summary>
        /// <param name="name">Optional name of the child flow in IOC registration.</param>
        /// <param name="index">Index of the node if multiple matches are found in the parent.  Defaults to first.</param>
        /// <returns>A child FlowComponentBuilder of this FlowComponentBuilder.</returns>
        IFlowComponentBuilder<T> ForChildFlow(string name = null, int index = 0);

        /// <summary>
        /// Returns an instance of FlowComponentBuilder representing the last child of the current builder.
        /// </summary>
        /// <returns>A child FlowComponentBuilder of this FlowComponentBuilder.</returns>
        IFlowComponentBuilder<T> ForLastChild();

        /// <summary>
        /// Returns an instance of FlowComponentBuilder representing the requested parent.
        /// </summary>
        /// <returns>A parent FlowComponentBuilder of this FlowComponentBuilder.</returns>
        IFlowComponentBuilder<T> ForParent();
    }
}