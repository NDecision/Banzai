using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banzai
{
    public interface IPipelineNode<T> : IMultiNode<T>
    {

    }

    public class PipelineNode<T> : MultiNode<T>, IPipelineNode<T>
    {
        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(ExecutionContext<T> context)
        {
            var results = new List<NodeResult<T>>();

            foreach (var childNode in Children)
            {
                var result = await childNode.ExecuteAsync(context).ConfigureAwait(false);

                results.Add(result);

                if (result.Status == NodeResultStatus.Failed && !context.EffectiveOptions.ContinueOnFailure)
                {
                    break;
                }
            }
            
            return AggregateNodeResults(results, context.EffectiveOptions);
        }
    }
}