using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Banzai.Factories
{
    
    /// <summary>
    /// Represents the definition of a flow component. Can represent a flow or a node.
    /// </summary>
    [DataContract]
    public class FlowComponent<T>
    {
        private List<FlowComponent<T>> _children = new List<FlowComponent<T>>();
        private IDictionary<string, object> _metaData = new Dictionary<string, object>();

        /// <summary>
        /// Type of the node if this represents a node or in a flow the subject type.
        /// </summary>
        [DataMember]
        public Type Type { get; set; }

        /// <summary>
        /// Id of the node or flow.  This can be used for identification in debugging.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Name of the node or flow.  This is used for IOC registration primarily.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether or not this component represents a flow or a node.
        /// </summary>
        [DataMember]
        public bool IsFlow { get; set; }

        /// <summary>
        /// Parent of this definition.
        /// </summary>
        public FlowComponent<T> Parent { get; set; }

        /// <summary>
        /// Children of this definition.
        /// </summary>
        [DataMember]
        public IReadOnlyList<FlowComponent<T>> Children
        {
            get { return _children; }
            set { _children = value.ToList(); }
        }

        /// <summary>
        /// Allows the ShouldExecuteFunc for this FlowComponent to be retrieved.
        /// </summary>
        public Func<IExecutionContext<T>, Task<bool>> ShouldExecuteFunc { get; private set; }

        /// <summary>
        /// Allows the ShouldExecuteBlock type for this FlowComponent to be retrieved.
        /// </summary>
        [DataMember]
        public Type ShouldExecuteBlockType { get; private set; }

        /// <summary>
        /// Allows metadata about this node to be set.
        /// </summary>
        [DataMember]
        public IDictionary<string, object> MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }

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
            child.Parent = this;

            return child;
        }

        /// <summary>
        /// Adds a ShouldExecuteFunc to the FlowComponent (to be added to the resultant node).
        /// </summary>
        /// <param name="shouldExecuteFunc">Function to add as ShouldExecute to the flowcomponent.</param>
        /// <returns>The current FlowComponent instance.</returns>
        protected internal FlowComponent<T> SetShouldExecute(Func<IExecutionContext<T>, Task<bool>> shouldExecuteFunc)
        {
            ShouldExecuteFunc = shouldExecuteFunc;
            return this;
        }

        /// <summary>
        /// Adds a ShouldExecuteBlock to the FlowComponent (to be added to the resultant node).
        /// </summary>
        /// <param name="blockType">ShouldExecuteBlock to add to the flowcomponent.</param>
        /// <returns>The current FlowComponent instance.</returns>
        protected internal FlowComponent<T> SetShouldExecute(Type blockType)
        {
            if (!typeof(IShouldExecuteBlock<T>).IsAssignableFrom(blockType))
                throw new ArgumentException("blockType must be assignable to IShouldExecuteBlock<T>.", "blockType");

            ShouldExecuteBlockType = blockType;
            return this;
        }

    }
}

