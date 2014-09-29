using System;
using System.Threading.Tasks;
using Banzai.Core.Utility;

namespace Banzai.Core
{

    /// <summary>
    /// The basic interface for a node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public interface INode<T>
    {
        ExecutionOptions LocalOptions { get; }

        NodeRunStatus Status { get; }

        Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> context);

        Task<bool> ShouldExecute(ExecutionContext<T> context);

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

        public async Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> sourceContext)
        {
            Guard.AgainstNullArgument("context", sourceContext);
            Guard.AgainstNullArgumentProperty("context", "Subject", sourceContext.Subject);

            var subject = sourceContext.Subject;
            var result = new NodeResult<T>(subject);

            ExecutionContext<T> context = PrepareExecutionContext(sourceContext, result);

            if (! await ShouldExecute(context))
                return result;

            Status = NodeRunStatus.Running;

            try
            {
                result.Status = await PerformExecuteAsync(context);
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

        public NodeRunStatus Status { get; private set; }
     }
}