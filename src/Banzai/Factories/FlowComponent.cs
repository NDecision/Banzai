using System;
using System.Collections.Generic;

namespace Banzai.Factories
{
    /// <summary>
    /// Represents the definition of a flow component.  Can represent a flow or a node.
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

        protected internal FlowComponent<T> AddChild(FlowComponent<T> child)
        {
            if(IsFlow && _children.Count > 0)
                throw new IndexOutOfRangeException("A flow can only have one root child.");

            _children.Add(child);

            return child;
        }

    }
}

