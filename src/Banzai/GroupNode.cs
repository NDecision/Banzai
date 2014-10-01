using System.Linq;
using System.Threading.Tasks;

namespace Banzai
{
    public interface IGroupNode<T> : IMultiNode<T>
    {
    }

    /// <summary>
    /// This node runs all children potentially simultaneously using Async's WhenAll.
    /// This is a good choice for multiple i/o operations.  The node will not complete until all children complete.
    /// </summary>
    public class GroupNode<T> : MultiNode<T>, IGroupNode<T>
    {

        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context)
        {
            Task<NodeResult<T>[]> aggregateTask = Task.WhenAll(Children.Select(x => x.ExecuteAsync(context)));
            NodeResult<T>[] results;

            try
            {
                results = await aggregateTask.ConfigureAwait(false);
            }
            catch
            {
                if(aggregateTask.Exception != null)
                    throw aggregateTask.Exception;

                throw;
            }

            return AggregateNodeResults(results, context.EffectiveOptions);
        }

    }
}