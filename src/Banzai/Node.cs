using System;
using System.Threading.Tasks;
using Banzai.Utility;

namespace Banzai
{

    /// <summary>
    /// The basic interface for a node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public interface INode<T>
    {
        /// <summary>
        /// Gets the local options associated with this node.  These options will apply only to the current node.
        /// </summary>
        ExecutionOptions LocalOptions { get; }

        /// <summary>
        /// Gets the current runstatus of this node.
        /// </summary>
        NodeRunStatus Status { get; }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult<T>> ExecuteAsync(T subject);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="sourceContext">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> sourceContext);

        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> ShouldExecute(ExecutionContext<T> context);

        /// <summary>
        /// Used to reset the node to a prerun state
        /// </summary>
        void Reset();
    }

    public abstract class Node<T> : INode<T>
    {
        protected Node()
        { }

        protected Node(ExecutionOptions localOptions)
        {
            LocalOptions = localOptions;
        }
        
        public ExecutionOptions LocalOptions { get; set; }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult<T>> ExecuteAsync(T subject)
        {
            return await ExecuteAsync(new ExecutionContext<T>(subject));
        }

        /// <summary>
        /// Used to kick off execution of a node with a specified execution context.
        /// </summary>
        /// <param name="sourceContext">ExecutionContext that includes a subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> sourceContext)
        {
            Guard.AgainstNullArgument("context", sourceContext);
            Guard.AgainstNullArgumentProperty("context", "Subject", sourceContext.Subject);

            if(Status != NodeRunStatus.NotRun)
                Reset();

            var subject = sourceContext.Subject;
            var result = new NodeResult<T>(subject);

            ExecutionContext<T> context = PrepareExecutionContext(sourceContext, result);

            if (! await ShouldExecute(context))
                return result;

            Status = NodeRunStatus.Running;

            try
            {
                result.Status = await PerformExecuteAsync(context);
                Status = NodeRunStatus.Completed;
            }
            catch (Exception ex)
            {
                Status = NodeRunStatus.Faulted;
                result.Status = NodeResultStatus.Failed;
                result.Exception = ex;

                if (sourceContext.EffectiveOptions.ThrowOnError)
                {
                    throw;
                }
            }

            return result;
        }

        protected virtual ExecutionContext<T> PrepareExecutionContext(ExecutionContext<T> context, NodeResult<T> currentResult)
        {
            context.AddResult(currentResult);
            if (LocalOptions != null)
                context.EffectiveOptions = LocalOptions;
            return context;
        }

        public virtual Task<bool> ShouldExecute(ExecutionContext<T> context)
        {
            return Task.FromResult(true);
        }

        protected abstract Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context);

        public virtual void Reset()
        {
            Status = NodeRunStatus.NotRun;
        }

        public NodeRunStatus Status { get; private set; }
     }
}