﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    ///     Class from which to derive custom pipeline nodes.
    ///     This node runs children serially as defined by their current order (as added).
    ///     This node is used for constructing pipelines. The node will not complete until all children complete or an error is
    ///     encountered.
    /// </summary>
    public abstract class PipelineNodeBase<T> : MultiNode<T>, IPipelineNodeBase<T>
    {
        /// <summary>
        ///     Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected override async Task<NodeResultStatus> ExecuteChildrenAsync(IExecutionContext<T> context)
        {
            var results = new List<NodeResult>();

            var effectiveOptions = GetEffectiveOptions(context.GlobalOptions);

            Logger.LogDebug("Running each child node in the pipeline sequentially.");
            foreach (var childNode in Children)
            {
                Logger.LogDebug("Running child node.");
                var result = await childNode.ExecuteAsync(context).ConfigureAwait(false);

                results.Add(result);

                if (result.Status == NodeResultStatus.Failed && !effectiveOptions.ContinueOnFailure ||
                    context.CancelProcessing) break;
            }

            return results.AggregateNodeResults(effectiveOptions);
        }
    }
}