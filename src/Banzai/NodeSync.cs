using System.Threading.Tasks;

namespace Banzai
{
    public abstract class NodeSync<T> : Node<T>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(PerformExecute(context));
        }

        protected abstract NodeResultStatus PerformExecute(ExecutionContext<T> context);
    }
}