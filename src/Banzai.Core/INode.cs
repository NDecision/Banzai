using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banzai.Core
{
    /// <summary>
    /// The basic interface for a node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public interface INode<T>
    {
        ExecutionOptions ExecutionOptions { get; }

        NodeRunStatus Status { get; }

        Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> context);

        bool ShouldExecute(T subject);

    }
}