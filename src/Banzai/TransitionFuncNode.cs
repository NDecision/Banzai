using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    ///     Node that allows a transition to another node type and specifies the transitions via functions.
    /// </summary>
    /// <typeparam name="TSource">Source node type.</typeparam>
    /// <typeparam name="TDestination">Destination node type.</typeparam>
    public class TransitionFuncNode<TSource, TDestination> : TransitionNode<TSource, TDestination>,
        ITransitionFuncNode<TSource, TDestination>
    {
        /// <summary>
        ///     Asynchronous function to transition the source to the destination.
        /// </summary>
        public Func<IExecutionContext<TSource>, Task<TDestination>> TransitionSourceFuncAsync { get; set; }

        /// <summary>
        ///     Asynchronous function to transition the destination result back to the source.
        /// </summary>
        public Func<IExecutionContext<TSource>, NodeResult, Task<TSource>> TransitionResultFuncAsync { get; set; }


        /// <summary>
        ///     Transitions from the source subject to the destination subject.
        /// </summary>
        /// <param name="sourceContext">The source execution context, including the subject.</param>
        /// <returns></returns>
        protected sealed override async Task<TDestination> TransitionSourceAsync(
            IExecutionContext<TSource> sourceContext)
        {
            if (TransitionSourceFuncAsync != null)
            {
                Logger.LogDebug("TransitionSourceFuncAsync exists, using this function.");
                return await TransitionSourceFuncAsync(sourceContext).ConfigureAwait(false);
            }

            Logger.LogDebug("TransitionSourceFuncAsync doesn't exist, returning default destination.");
            return default;
        }


        /// <summary>
        ///     Transitions the source based on the child result to prepare for return to the source flow.
        /// </summary>
        /// <param name="sourceContext">Context including the source subject.</param>
        /// <param name="result">The result of the destination node.</param>
        /// <returns>The transitioned subject.</returns>
        protected sealed override async Task<TSource> TransitionResultAsync(IExecutionContext<TSource> sourceContext,
            NodeResult result)
        {
            if (TransitionResultFuncAsync != null)
            {
                Logger.LogDebug("TransitionResultFuncAsync exists, using this function.");
                return await TransitionResultFuncAsync(sourceContext, result).ConfigureAwait(false);
            }

            Logger.LogDebug("TransitionResultFuncAsync doesn't exist, returning original subject.");
            return sourceContext.Subject;
        }
    }
}