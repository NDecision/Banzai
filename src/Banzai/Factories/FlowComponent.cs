using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banzai.Factories
{
    /// <summary>
    /// Represents the definition of a flow component. Can represent a flow or a node.
    /// </summary>
    public class FlowComponent<T>
    {
        private readonly List<FlowComponent<T>> _children = new List<FlowComponent<T>>();

        /// <summary>
        /// Type of the node if this represents a node or in a flow the subject type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Name of the node or flow.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether or not this component represents a flow or a node.
        /// </summary>
        public bool IsFlow { get; set; }

        /// <summary>
        /// Children of this definition.
        /// </summary>
        public IReadOnlyList<FlowComponent<T>> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Allows the ShouldExecuteFunc for this FlowComponent to be retrieved.
        /// </summary>
        public Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; private set; }

        /// <summary>
        /// Allows the ShouldExecuteFuncAsync for this FlowComponent to be retrieved.
        /// </summary>
        public Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; private set; }

        /// <summary>
        /// Adds a child to this FlowComponent.
        /// </summary>
        /// <param name="child">Child to add.</param>
        /// <returns>Updated FlowComponent.</returns>
        protected internal FlowComponent<T> AddChild(FlowComponent<T> child)
        {
            if(IsFlow && _children.Count > 0)
                throw new IndexOutOfRangeException("A flow can only have one root child.");

            _children.Add(child);

            return child;
        }

        /// <summary>
        /// Adds a ShouldExecute to the FlowComponent (to be added to the resultant node).
        /// </summary>
        /// <param name="shouldExecuteFunc">Function to add as ShouldExecute to the flowcomponent.</param>
        /// <returns>The current FlowComponent instance.</returns>
        protected internal FlowComponent<T> SetShouldExecute(Func<ExecutionContext<T>, bool> shouldExecuteFunc)
        {
            ShouldExecuteFunc = shouldExecuteFunc;
            return this;
        }

        /// <summary>
        /// Adds a ShouldExecuteAsync to the FlowComponent (to be added to the resultant node).
        /// </summary>
        /// <param name="shouldExecuteFuncAsync">Function to add as ShouldExecute to the flowcomponent.</param>
        /// <returns>The current FlowComponent instance.</returns>
        protected internal FlowComponent<T> SetShouldExecuteAsync(Func<ExecutionContext<T>, Task<bool>> shouldExecuteFuncAsync)
        {
            ShouldExecuteFuncAsync = shouldExecuteFuncAsync;
            return this;
        }

    }
}

