using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banzai.Core
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
                var result = await childNode.ExecuteAsync(context);

                results.Add(result);

                if (result.Status == NodeResultStatus.Failed && !context.EffectiveOptions.ContinueOnError)
                {
                    break;
                }
            }

            return AggregateNodeResults(results);
        }
    }
}