using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banzai
{
    /// <summary>
    /// This node runs children serially as defined by their current order (as added).
    /// This node is used for constructing pipelines. The node will not complete until all children complete or an error is encountered.
    /// </summary>
    public interface IPipelineNode<T> : IMultiNode<T>
    {

    }

    /// <summary>
    /// This node runs children serially as defined by their current order (as added).
    /// This node is used for constructing pipelines. The node will not complete until all children complete or an error is encountered.
    /// </summary>
    public class PipelineNode<T> : MultiNode<T>, IPipelineNode<T>
    {
        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
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