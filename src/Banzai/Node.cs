using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banzai.Logging;
using Banzai.Utility;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    ///     The basic class for a functional node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public abstract class Node<T> : INode<T>
    {
        private string _id;
        private bool _processManyMode;

        /// <summary>
        ///     Creates a new Node.
        /// </summary>
        protected Node()
        {
            _id = GetType().FullName;
        }

        /// <summary>
        ///     Creates a new Node with local options to override global options.
        /// </summary>
        /// <param name="localOptions">Local options to override global options.</param>
        protected Node(ExecutionOptions localOptions) : this()
        {
            LocalOptions = localOptions;
        }

        /// <summary>
        ///     Holds a reference to the result of the most recent node execution.
        /// </summary>
        public NodeResult Result { get; private set; }

        /// <summary>
        ///     Id of the current node, can be set to help with debugging.  Defaults to the node type name.
        /// </summary>
        public virtual string Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        ///     Id of the current flow, can be set to help with debugging.
        /// </summary>
        public virtual string FlowId { get; set; }

        /// <summary>
        ///     Local options which override the global options when this Node is run.  Applies only to the current node.
        /// </summary>
        public ExecutionOptions LocalOptions { get; set; }

        /// <summary>
        ///     Current run status of this node.
        /// </summary>
        public NodeRunStatus Status { get; private set; }

        /// <summary>
        ///     Metadata applied to the node.
        /// </summary>
        public dynamic CustomData { get; set; }

        /// <summary>
        ///     Logger used to write to the log from this node.
        /// </summary>
        public ILogger Logger => LoggerProvider.CreateLogger(this);

        /// <summary>
        ///     Resets the current node to unrun state.
        /// </summary>
        public virtual void Reset()
        {
            Logger.LogDebug("Resetting the node.");
            Status = NodeRunStatus.NotRun;
        }

        /// <summary>
        ///     Gets or sets the block to define if this node should be executed.
        /// </summary>
        public object ShouldExecuteBlock { get; set; }

        /// <summary>
        ///     Gets or sets the async function to call to determine if this node should be executed.
        /// </summary>
        public Func<IExecutionContext<object>, Task<bool>> ShouldExecuteFunc { get; set; }

        /// <summary>
        ///     Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual Task<bool> ShouldExecuteAsync(IExecutionContext<T> context)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///     Used to kick off execution of a node with a default execution context for all subjects in a serial manner.
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        public async Task<NodeResult> ExecuteManySeriallyAsync(IEnumerable<T> subjects, ExecutionOptions options = null)
        {
            Guard.AgainstNullArgument("subjects", subjects);

            var nodeTimer = new NodeTimer();

            try
            {
                nodeTimer.LogStart(Logger, this, "ExecuteManySeriallyAsync");

                _processManyMode = true;

                var subjectList = subjects.ToList();

                Result = new NodeResult(default(T), Id, FlowId);

                if (subjectList.Count == 0)
                    return Result;

                if (options == null)
                    options = new ExecutionOptions();

                foreach (var subject in subjectList)
                {
                    try
                    {
                        Logger.LogDebug("Running all subjects asynchronously in a serial manner.");

                        var result = await ExecuteAsync(new ExecutionContext<T>(subject, options))
                            .ConfigureAwait(false);

                        Result.AddChildResult(result);
                    }
                    catch (Exception)
                    {
                        if (options.ThrowOnError)
                        {
                            throw;
                        }

                        if (!options.ContinueOnFailure)
                        {
                            break;
                        }
                    }
                }

                ProcessExecuteManyResults(options);

                return Result;
            }
            finally
            {
                _processManyMode = false;
                nodeTimer.LogStop(Logger, this, "ExecuteAsync");
            }
        }

        /// <summary>
        ///     Used to kick off execution of a node with a default execution context for all subjects using Async WhenAll
        ///     semantics internally .
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        public async Task<NodeResult> ExecuteManyAsync(IEnumerable<T> subjects, ExecutionOptions options = null)
        {
            Guard.AgainstNullArgument("subjects", subjects);

            var nodeTimer = new NodeTimer();

            try
            {
                nodeTimer.LogStart(Logger, this, "ExecuteManyAsync");
                _processManyMode = true;
                Result = new NodeResult(default(T), Id, FlowId);

                var subjectList = subjects.ToList();

                if (subjectList.Count == 0)
                    return Result;

                if (options == null)
                    options = new ExecutionOptions();

                Logger.LogDebug("Running all subjects asynchronously.");

                Task aggregateTask = null;
                try
                {
                    var resultsQueue = new ConcurrentQueue<NodeResult>();
                    aggregateTask = subjectList.ForEachAsync(options.DegreeOfParallelism,
                        async x => resultsQueue.Enqueue(await ExecuteAsync(new ExecutionContext<T>(x, options))));

                    await aggregateTask;

                    Result.AddChildResults(resultsQueue);
                }
                catch
                {
                    if (options.ThrowOnError)
                    {
                        if (aggregateTask?.Exception != null)
                            throw aggregateTask.Exception;

                        throw;
                    }
                }

                ProcessExecuteManyResults(options);
                return Result;
            }
            finally
            {
                _processManyMode = false;
                nodeTimer.LogStop(Logger, this, "ExecuteAsync");
            }
        }

        /// <summary>
        ///     Used to kick off execution of a node with a new default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult> ExecuteAsync(T subject)
        {
            return await ExecuteAsync(new ExecutionContext<T>(subject)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Used to kick off execution of a node with a specified execution context.
        /// </summary>
        /// <param name="sourceContext">ExecutionContext that includes a subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult> ExecuteAsync(IExecutionContext<T> sourceContext)
        {
            Guard.AgainstNullArgument("context", sourceContext);
            Guard.AgainstNullArgumentProperty("context", "Subject", sourceContext.Subject);

            var nodeTimer = new NodeTimer();

            try
            {
                nodeTimer.LogStart(Logger, this, "ExecuteAsync");

                if (Status != NodeRunStatus.NotRun)
                {
                    Logger.LogDebug("Status does not equal 'NotRun', resetting the node before execution");
                    Reset();
                }

                var subject = sourceContext.Subject;
                var result = new NodeResult(subject, Id, FlowId);
                if (!_processManyMode)
                    Result = result;

                var context = PrepareExecutionContext(sourceContext, result);

                OnBeforeExecute(context);

                if (!context.CancelProcessing)
                {
                    var effectiveOptions = GetEffectiveOptions(context.GlobalOptions);

                    if (!await ShouldExecuteInternalAsync(context).ConfigureAwait(false))
                    {
                        Logger.LogInformation("ShouldExecute returned false, skipping execution");
                        return result;
                    }

                    Status = NodeRunStatus.Running;
                    Logger.LogDebug("Executing the node");

                    try
                    {
                        result.Status = await PerformExecuteAsync(context).ConfigureAwait(false);
                        //Reset the subject in case it was changed.
                        result.Subject = context.Subject;
                        Status = NodeRunStatus.Completed;
                        Logger.LogInformation("Node completed execution, status is {0}", result.Status);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Node erred during execution, status is Failed", ex);
                        Status = NodeRunStatus.Faulted;
                        result.Subject = context.Subject;
                        result.Status = NodeResultStatus.Failed;
                        result.Exception = ex;

                        if (effectiveOptions.ThrowOnError)
                        {
                            throw;
                        }
                    }

                    OnAfterExecute(context);
                }

                sourceContext.CancelProcessing = context.CancelProcessing;

                return result;
            }
            finally
            {
                nodeTimer.LogStop(Logger, this, "ExecuteAsync");
            }
        }

        /// <summary>
        ///     Gets the current effective options of this node based on the passed execution options and its own local options.
        /// </summary>
        /// <param name="globalOptions">Current global options (typically from the current ExecutionContext)</param>
        /// <returns>Effective options applied to this node when it executes.</returns>
        public ExecutionOptions GetEffectiveOptions(ExecutionOptions globalOptions)
        {
            if (LocalOptions != null)
            {
                Logger.LogDebug(
                    "Local options detected, merging with global settings. Local Options - ContinueOnFailure:{0}, ThrowOnError:{1}",
                    LocalOptions.ContinueOnFailure, LocalOptions.ThrowOnError);
                return LocalOptions;
            }

            Logger.LogDebug(
                "Local options not present, defaulting to global settings. Global Options - ContinueOnFailure:{0}, ThrowOnError:{1}",
                globalOptions.ContinueOnFailure, globalOptions.ThrowOnError);

            return globalOptions;
        }

        /// <summary>
        ///     Method to override to provide functionality to the current node.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }

        /// <summary>
        ///     Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="result">The result reference to add to the current context.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected virtual IExecutionContext<T> PrepareExecutionContext(IExecutionContext<T> context, NodeResult result)
        {
            Logger.LogDebug("Preparing the execution context for execution.");
            context.AddResult(result);

            return context;
        }

        /// <summary>
        ///     Called before the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnBeforeExecute(IExecutionContext<T> context)
        {
        }

        /// <summary>
        ///     Called after the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnAfterExecute(IExecutionContext<T> context)
        {
        }

        private void ProcessExecuteManyResults(ExecutionOptions options)
        {
            Result.Status = Result.ChildResults.AggregateNodeResults(options);

            var exceptions = Result.GetFailExceptions().ToList();
            if (exceptions.Count > 0)
            {
                Logger.LogInformation("Child executions returned {0} exceptions.", exceptions.Count);
                Result.Exception = exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions);
            }
        }

        private async Task<bool> ShouldExecuteInternalAsync(IExecutionContext<T> context)
        {
            var shouldExecute = true;

            if (ShouldExecuteFunc != null)
                shouldExecute = await ShouldExecuteFunc((IExecutionContext<object>) context).ConfigureAwait(false);

            if (shouldExecute && ShouldExecuteBlock != null)
                shouldExecute = await ((IShouldExecuteBlock<T>) ShouldExecuteBlock).ShouldExecuteAsync(context)
                    .ConfigureAwait(false);

            if (shouldExecute)
                shouldExecute = await ShouldExecuteAsync(context).ConfigureAwait(false);

            return shouldExecute;
        }
    }
}