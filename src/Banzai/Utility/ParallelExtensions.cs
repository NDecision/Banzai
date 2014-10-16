using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banzai.Utility
{
    /// <summary>
    /// Extensions to help with parallel operations.
    /// </summary>
    internal static class ParallelExtensions
    {
        /// <summary>
        /// Performs the function asyncrhonously using the specified degree of parallelism.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable to operate on.</typeparam>
        /// <param name="source">Source to operate on.</param>
        /// <param name="degreeOfParallelism">Number of simultaneously executing operations.</param>
        /// <param name="func">Function to execute.</param>
        /// <returns>A whenall task to await.</returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> func)
        {
            if (degreeOfParallelism <= 1)
                return Task.WhenAll(source.Select(func));

            var partitions = Partitioner.Create(source).GetPartitions(degreeOfParallelism);
            var tasks = partitions.Select(async partition =>
            {
                using (partition)
                    while (partition.MoveNext())
                        await func(partition.Current);
            });
            return Task.WhenAll(tasks);
        }
    }
}