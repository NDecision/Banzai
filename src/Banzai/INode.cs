using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    /// The basic interface for a node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public interface INode<in T>
    {
        /// <summary>
        /// Id of the current node, can be set to help with debugging.  Defaults to the node type name.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Id of the current flow, can be set to help with debugging.
        /// </summary>
        string FlowId { get; set; }


        /// <summary>
        /// Gets the local options associated with this node.  These options will apply only to the current node.
        /// </summary>
        ExecutionOptions LocalOptions { get; }

        /// <summary>
        /// Gets the current runstatus of this node.
        /// </summary>
        NodeRunStatus Status { get; }

        /// <summary>
        /// Metadata applied to the node.
        /// </summary>
        dynamic CustomData { get; set; }

        /// <summary>
        /// Gets the current log writer
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets or sets the async function to call to determine if this node should be executed.
        /// </summary>
        Func<IExecutionContext<object>, Task<bool>> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Gets or sets the block to define if this node should be executed.
        /// </summary>
        object ShouldExecuteBlock { get; set; }

        /// <summary>
        /// Used to reset the node to a prerun state
        /// </summary>
        void Reset();


        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>Bool indicating if the current node should be run.</returns>
        Task<bool> ShouldExecuteAsync(IExecutionContext<T> context);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult> ExecuteAsync(T subject);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="sourceContext">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult> ExecuteAsync(IExecutionContext<T> sourceContext);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context for all subjects using Async WhenAll semantics internally .
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        Task<NodeResult> ExecuteManyAsync(IEnumerable<T> subjects, ExecutionOptions options = null);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context for all subjects in a serial manner.
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        Task<NodeResult> ExecuteManySeriallyAsync(IEnumerable<T> subjects, ExecutionOptions options = null);

    }
}