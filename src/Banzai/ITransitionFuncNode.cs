using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    ///     Interface for a Node that allows a transition to another node type and specifies the transitions via functions.
    /// </summary>
    /// <typeparam name="TSource">Source node type.</typeparam>
    /// <typeparam name="TDestination">Destination node type.</typeparam>
    public interface ITransitionFuncNode<TSource, TDestination> : ITransitionNode<TSource, TDestination>
    {
        /// <summary>
        ///     Asynchronous function to transition the source to the destination.
        /// </summary>
        Func<IExecutionContext<TSource>, Task<TDestination>> TransitionSourceFuncAsync { get; set; }

        /// <summary>
        ///     Asynchronous function to transition the destination result back to the source.
        /// </summary>
        Func<IExecutionContext<TSource>, NodeResult, Task<TSource>> TransitionResultFuncAsync { get; set; }
    }
}