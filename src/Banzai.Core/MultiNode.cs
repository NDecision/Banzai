using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banzai.Core
{
    /// <summary>
    /// This node runs all children simultaneously using async, but does not use the parallel library to process them.
    /// This is a good choice for multiple i/o operations.  The node will not complete until all children complete.
    /// </summary>
    public class MultiNode<T> : GroupNode<T>
    {
        protected override async Task<NodeResultStatus> PerformExecuteAsync(T subject)
        {
            if (Children == null || Children.Count == 0)
            {
                return NodeResultStatus.NotRun;
            }

            Task<NodeResult<T>[]> aggregateTask = Task.WhenAll(Children.Select(x => x.ExecuteAsync(subject)));
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

            return AggregateNodeResults(results);

        }

        
    }
}