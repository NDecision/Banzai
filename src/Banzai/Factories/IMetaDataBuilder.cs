using System.Collections.Generic;

namespace Banzai.Factories
{
    /// <summary>
    /// A metadata builder to apply metadata to a node.
    /// </summary>
    public interface IMetaDataBuilder
    {
        /// <summary>
        /// Applies metadata to the passed node.
        /// </summary>
        /// <typeparam name="T">Type of the node's subject.</typeparam>
        /// <param name="node">The node to apply metadata to.</param>
        /// <param name="metaData">Metadata to apply.</param>
        void Apply<T>(INode<T> node, IDictionary<string, object> metaData);
    }
}