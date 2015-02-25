using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// Extensions to make adding should execute functions to an INode type safe.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Adds a ShouldExecuteFunc to the INode.
        /// </summary>
        /// <typeparam name="T">Type of the subject the node acts upon.</typeparam>
        /// <param name="node">Node to add ShouldExecute to.</param>
        /// <param name="shouldExecuteFunc">Strongly typed ShouldExecuteFunc.</param>
        /// <returns>The INode with the function added.</returns>
        public static INode<T> AddShouldExecute<T>(this INode<T> node,
            Func<IExecutionContext<T>, Task<bool>> shouldExecuteFunc)
        {
            node.ShouldExecuteFunc = context => shouldExecuteFunc((IExecutionContext<T>) context);
            return node;
        }


        /// <summary>
        /// Adds a ShouldExecuteFunc to the INode.
        /// </summary>
        /// <typeparam name="T">Type of the subject the node acts upon.</typeparam>
        /// <param name="node">Node to add ShouldExecute to.</param>
        /// <param name="shouldExecuteBlock">Strongly typed ShouldExecuteBlock.</param>
        /// <returns>The INode with the function added.</returns>
        public static INode<T> AddShouldExecuteBlock<T>(this INode<T> node,
            IShouldExecuteBlock<T> shouldExecuteBlock)
        {
            node.ShouldExecuteBlock = shouldExecuteBlock;
            return node;
        }
    }
}