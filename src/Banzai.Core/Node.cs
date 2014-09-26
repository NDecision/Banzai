using System;
using System.Threading.Tasks;

namespace Banzai.Core
{

    public abstract class Node<T> : INode<T>
    {
        protected Node()
        { }

        protected Node(ExecutionOptions options)
        {
            ExecutionOptions = options;
        }
        
        public ExecutionOptions ExecutionOptions { get; set; }

        public async Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> context)
        {
            var subject = context.Subject;
            var result = new NodeResult<T>(subject);

            if (!ShouldExecute(subject))
                return result;

            Status = NodeRunStatus.Running;

            try
            {
                result.Status = await PerformExecuteAsync(subject);
            }
            catch (Exception ex)
            {
                Status = NodeRunStatus.Faulted;
                result.Status = NodeResultStatus.Failed;
                result.Exception = ex;

                if (ExecutionOptions.ThrowOnError)
                {
                    throw;
                }
            }

            return result;
        }

        public virtual bool ShouldExecute(T subject)
        {
            return true;
        }

        protected abstract Task<NodeResultStatus> PerformExecuteAsync(T subject);

        public NodeRunStatus Status { get; private set; }
     }
}