using System.Collections.Generic;

namespace Banzai
{
    /// <summary>
    /// Extension methods for Node results.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Aggregates the child results of a MultiNode into a summary result status.
        /// </summary>
        /// <param name="results">Results to aggregate.</param>
        /// <param name="options">Execution options to consider during aggregation.</param>
        /// <returns>Summary NodeResultStatus.</returns>
        public static NodeResultStatus AggregateNodeResults<T>(this IEnumerable<NodeResult<T>> results, ExecutionOptions options)
        {
            bool hasFailure = false;
            bool hasSuccess = false;
            bool hasSuccessWithErrors = false;

            foreach (var nodeResult in results)
            {
                if (!hasSuccessWithErrors && nodeResult.Status == NodeResultStatus.SucceededWithErrors)
                {
                    hasSuccessWithErrors = true;
                }
                else if (!hasFailure && nodeResult.Status == NodeResultStatus.Failed)
                {
                    hasFailure = true;
                }
                else if (!hasSuccess && nodeResult.Status == NodeResultStatus.Succeeded)
                {
                    hasSuccess = true;
                }
                if (hasSuccess && hasFailure)
                {
                    hasSuccessWithErrors = true;
                    break;
                }
            }

            if (hasSuccessWithErrors)
            {
                if (hasFailure && !options.ContinueOnFailure)
                {
                    return NodeResultStatus.Failed;
                }

                return NodeResultStatus.SucceededWithErrors;
            }

            if (hasSuccess)
                return NodeResultStatus.Succeeded;
            if (hasFailure)
                return NodeResultStatus.Failed;

            return NodeResultStatus.NotRun;
        }
    }
}