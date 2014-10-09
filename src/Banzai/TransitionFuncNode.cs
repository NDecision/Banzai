using System;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// Interface for a Node that allows a transition to another node type and specifies the transitions via functions.
    /// </summary>
    /// <typeparam name="TSource">Source node type.</typeparam>
    /// <typeparam name="TDestination">Destination node type.</typeparam>
    public interface ITransitionFuncNode<TSource, TDestination> : ITransitionNode<TSource, TDestination>
    {
        /// <summary>
        /// Asynchronous function to transition the source to the destination.
        /// </summary>
        Func<ExecutionContext<TSource>, Task<TDestination>> TransitionSourceFuncAsync { get; set; }

        /// <summary>
        /// Synchronous function to transition the source to the destination.
        /// </summary>
        Func<ExecutionContext<TSource>, TDestination> TransitionSourceFunc { get; set; }

        /// <summary>
        /// Asynchronous function to transition the destination result back to the source.
        /// </summary>
        Func<ExecutionContext<TSource>, NodeResult<TDestination>, Task<TSource>> TransitionResultFuncAsync { get; set; }

        /// <summary>
        /// Asynchronous function to transition the destination result back to the source.
        /// </summary>
        Func<ExecutionContext<TSource>, NodeResult<TDestination>, TSource> TransitionResultFunc { get; set; }
    }

    /// <summary>
    /// Node that allows a transition to another node type and specifies the transitions via functions.
    /// </summary>
    /// <typeparam name="TSource">Source node type.</typeparam>
    /// <typeparam name="TDestination">Destination node type.</typeparam>
    public class TransitionFuncNode<TSource,TDestination> : TransitionNode<TSource, TDestination>
    {
        /// <summary>
        /// Asynchronous function to transition the source to the destination.
        /// </summary>
        public Func<ExecutionContext<TSource>, Task<TDestination>> TransitionSourceFuncAsync { get; set; }

        /// <summary>
        /// Synchronous function to transition the source to the destination.
        /// </summary>
        public Func<ExecutionContext<TSource>, TDestination> TransitionSourceFunc { get; set; }

        /// <summary>
        /// Asynchronous function to transition the destination result back to the source.
        /// </summary>
        public Func<ExecutionContext<TSource>, NodeResult<TDestination>, Task<TSource>> TransitionResultFuncAsync { get; set; }

        /// <summary>
        /// Asynchronous function to transition the destination result back to the source.
        /// </summary>
        public Func<ExecutionContext<TSource>, NodeResult<TDestination>, TSource> TransitionResultFunc { get; set; }


        /// <summary>
        /// Transitions from the source subject to the destination subject.
        /// </summary>
        /// <param name="sourceContext">The source execution context, including the subject.</param>
        /// <returns></returns>
        protected async override sealed Task<TDestination> TransitionSourceAsync(ExecutionContext<TSource> sourceContext)
        {
            if (TransitionSourceFuncAsync != null)
            {
                LogWriter.Debug("TransitionSourceFuncAsync exists, using this function.");
                return await TransitionSourceFuncAsync(sourceContext).ConfigureAwait(false);
            }
            LogWriter.Debug("TransitionSourceFuncAsync doesn't exist.");
            return TransitionSource(sourceContext);
        }

        /// <summary>
        /// Transitions from the source subject to the destination subject.
        /// </summary>
        /// <param name="sourceContext">The source execution context, including the subject.</param>
        /// <returns></returns>
        protected override sealed TDestination TransitionSource(ExecutionContext<TSource> sourceContext)
        {
            if (TransitionSourceFunc != null)
            {
                LogWriter.Debug("TransitionSourceFunc exists, using this function.");
                return TransitionSourceFunc(sourceContext);
            }
            LogWriter.Debug("TransitionSourceFunc doesn't exist, returning default destination.");
            return default(TDestination);
        }

        /// <summary>
        /// Transitions the source based on the child result to prepare for return to the source flow. 
        /// </summary>
        /// <param name="sourceContext">Context including the source subject.</param>
        /// <param name="result">The result of the destination node.</param>
        /// <returns>The transitioned subject.</returns>
        protected async override sealed Task<TSource> TransitionResultAsync(ExecutionContext<TSource> sourceContext, NodeResult<TDestination> result)
        {
            if (TransitionResultFuncAsync != null)
            {
                LogWriter.Debug("TransitionResultFuncAsync exists, using this function.");
                return await TransitionResultFuncAsync(sourceContext, result).ConfigureAwait(false);
            }
            LogWriter.Debug("TransitionResultFuncAsync doesn't exist.");
            return TransitionResult(sourceContext, result);
        }

        /// <summary>
        /// Transitions the source based on the child result to prepare for return to the source flow. 
        /// </summary>
        /// <param name="sourceContext">Context including the source subject.</param>
        /// <param name="result">The result of the destination node.</param>
        /// <returns>The transitioned subject.</returns>
        protected override sealed TSource TransitionResult(ExecutionContext<TSource> sourceContext, NodeResult<TDestination> result)
        {
            if (TransitionResultFunc != null)
            {
                LogWriter.Debug("TransitionResultFunc exists, using this function.");
                return TransitionResultFunc(sourceContext, result);
            }
            LogWriter.Debug("TransitionResultFunc doesn't exist, returning original subject.");
            return sourceContext.Subject;
        }
    }
}