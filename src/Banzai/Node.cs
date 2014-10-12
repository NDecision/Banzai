using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banzai.Logging;
using Banzai.Utility;

namespace Banzai
{

    /// <summary>
    /// The basic interface for a node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public interface INode<T> 
    {
        /// <summary>
        /// Gets the local options associated with this node.  These options will apply only to the current node.
        /// </summary>
        ExecutionOptions LocalOptions { get; }

        /// <summary>
        /// Gets the current runstatus of this node.
        /// </summary>
        NodeRunStatus Status { get; }

        /// <summary>
        /// Gets the current log writer
        /// </summary>
        ILogWriter LogWriter { get; }

        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>Bool indicating if the current node should be run.</returns>
        bool ShouldExecute(IExecutionContext<T> context);

        /// <summary>
        /// Gets or sets the function to define if this node should be executed.
        /// </summary>
        Func<IExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Gets or sets the async function to call to determine if this node should be executed.
        /// </summary>
        Func<IExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

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

        /// <summary>
        /// Used to reset the node to a prerun state
        /// </summary>
        void Reset();

    }

    /// <summary>
    /// The basic class for a functional node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public abstract class Node<T> : INode<T>
    {

        /// <summary>
        /// Creates a new Node.
        /// </summary>
        protected Node()
        {
        }

        /// <summary>
        /// Creates a new Node with local options to override global options.
        /// </summary>
        /// <param name="localOptions">Local options to override global options.</param>
        protected Node(ExecutionOptions localOptions) : this()
        {
            LocalOptions = localOptions;
        }
        
        /// <summary>
        /// Local options which override the global options when this Node is run.  Applies only to the current node.
        /// </summary>
        public ExecutionOptions LocalOptions { get; set; }

        /// <summary>
        /// Current run status of this node.
        /// </summary>
        public NodeRunStatus Status { get; private set; }

        /// <summary>
        /// LogWriter used to write to the log from this node.
        /// </summary>
        public ILogWriter LogWriter { get { return Logging.LogWriter.GetLogger(this); } }

        /// <summary>
        /// Resets the current node to unrun state.
        /// </summary>
        public virtual void Reset()
        {
            LogWriter.Debug("Resetting the node.");
            Status = NodeRunStatus.NotRun;
        }

        /// <summary>
        /// Gets or sets the function to define if this node should be executed.
        /// </summary>
        public Func<IExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Gets or sets the async function to call to determine if this node should be executed.
        /// </summary>
        public Func<IExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

        /// <summary>
        /// Determines if the current node should execute with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual bool ShouldExecute(IExecutionContext<T> context)
        {
            return true;
        }

        /// <summary>
        /// Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual Task<bool> ShouldExecuteAsync(IExecutionContext<T> context)
        {
            return Task.FromResult(ShouldExecute(context));
        }

        /// <summary>
        /// Internal method that combines all of the ShouldExecute functions and methods.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if the task should run.</returns>
        private async Task<bool> ShouldExecuteInternal(IExecutionContext<T> context)
        {
            if (ShouldExecuteFuncAsync != null)
                return await ShouldExecuteFuncAsync(context).ConfigureAwait(false);

            if (ShouldExecuteFunc != null)
                return ShouldExecuteFunc(context);

            return await ShouldExecuteAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult> ExecuteAsync(T subject)
        {
            return await ExecuteAsync(new ExecutionContext<T>(subject));
        }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context for all subjects in a serial manner.
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        public async Task<NodeResult> ExecuteManySeriallyAsync(IEnumerable<T> subjects, ExecutionOptions options = null)
        {
            Guard.AgainstNullArgument("subjects", subjects);

            var subjectList = subjects.ToList();

            var aggregateResult = new NodeResult(default(T));

            if (subjectList.Count == 0)
                return aggregateResult;

            if (options == null)
                options = new ExecutionOptions();

            foreach (var subject in subjectList)
            {
                try
                {
                    LogWriter.Debug("Running all subjects asynchronously.");
                    NodeResult result = await ExecuteAsync(new ExecutionContext<T>(subject, options));

                    aggregateResult.AddChildResult(result);
                }
                catch(Exception)
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

            ProcessExecuteManyResults(options, aggregateResult);

            return aggregateResult;
        }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context for all subjects using Async WhenAll semantics internally .
        /// </summary>
        /// <param name="subjects">Subject to be moved through the node.</param>
        /// <param name="options">Execution options to apply to running this enumerable of subjects.</param>
        /// <returns>An aggregated NodeResult.</returns>
        public async Task<NodeResult> ExecuteManyAsync(IEnumerable<T> subjects, ExecutionOptions options = null)
        {
            Guard.AgainstNullArgument("subjects", subjects);

            var subjectList = subjects.ToList();

            var aggregateResult = new NodeResult(default(T));

            if (subjectList.Count == 0)
                return aggregateResult;

            if(options == null)
                options = new ExecutionOptions();

            Task<NodeResult[]> aggregateTask = null;

            try
            {
                LogWriter.Debug("Running all subjects asynchronously.");
                aggregateTask = Task.WhenAll(subjectList.Select(x => ExecuteAsync(new ExecutionContext<T>(x, options))));
                NodeResult[] results = await aggregateTask.ConfigureAwait(false);

                aggregateResult.AddChildResults(results);
            }
            catch
            {
                if (options.ThrowOnError)
                {
                    if (aggregateTask.Exception != null)
                        throw aggregateTask.Exception;

                    throw;
                }
            }

            ProcessExecuteManyResults(options, aggregateResult);

            return aggregateResult;
        }

        /// <summary>
        /// Used to kick off execution of a node with a specified execution context.
        /// </summary>
        /// <param name="sourceContext">ExecutionContext that includes a subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult> ExecuteAsync(IExecutionContext<T> sourceContext)
        {
            Guard.AgainstNullArgument("context", sourceContext);
            Guard.AgainstNullArgumentProperty("context", "Subject", sourceContext.Subject);

            if (Status != NodeRunStatus.NotRun)
            {
                LogWriter.Debug("Status does not equal 'NotRun', resetting the node before execution");
                Reset();
            }

            var subject = sourceContext.Subject;
            var result = new NodeResult(subject);

            IExecutionContext<T> context = PrepareExecutionContext(sourceContext, result);

            var effectiveOptions = GetEffectiveOptions(context.GlobalOptions);

            OnBeforeExecute(context);

            if (! await ShouldExecuteInternal(context).ConfigureAwait(false))
            {
                LogWriter.Info("ShouldExecute returned false, skipping execution");
                return result;
            }

            Status = NodeRunStatus.Running;
            LogWriter.Debug("Executing the node");

            try
            {
                result.Status = await PerformExecuteAsync(context).ConfigureAwait(false);
                //Reset the subject in case it was changed.
                result.Subject = context.Subject;
                Status = NodeRunStatus.Completed;
                LogWriter.Info("Node completed execution, status is {0}", result.Status);
            }
            catch (Exception ex)
            {
                LogWriter.Error("Node erred during execution, status is Failed", ex);
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

            return result;
        }

        /// <summary>
        /// Gets the current effective options of this node based on the passed execution options and its own local options.
        /// </summary>
        /// <param name="globalOptions">Current global options (typically from the current ExecutionContext)</param>
        /// <returns>Effective options applied to this node when it executes.</returns>
        public ExecutionOptions GetEffectiveOptions(ExecutionOptions globalOptions)
        {
            if (LocalOptions != null)
            {
                LogWriter.Debug("Local options detected, merging with global settings. Local Options - ContinueOnFailure:{0}, ThrowOnError:{1}",
                    LocalOptions.ContinueOnFailure, LocalOptions.ThrowOnError);
                return LocalOptions;
            }
            LogWriter.Debug("Local options not present, defaulting to global settings. Global Options - ContinueOnFailure:{0}, ThrowOnError:{1}",
                    globalOptions.ContinueOnFailure, globalOptions.ThrowOnError);

            return globalOptions;
        }

        /// <summary>
        /// Method to override to provide functionality to the current node with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual NodeResultStatus PerformExecute(IExecutionContext<T> context)
        {
            return NodeResultStatus.Succeeded;
        }

        /// <summary>
        /// Method to override to provide functionality to the current node.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<T> context)
        {
            return Task.FromResult(PerformExecute(context));
        }

        /// <summary>
        /// Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="currentResult">A referene to the result of the current node.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected virtual IExecutionContext<T> PrepareExecutionContext(IExecutionContext<T> context, NodeResult currentResult)
        {
            LogWriter.Debug("Preparing the execution context for execution.");
            context.AddResult(currentResult);

            return context;
        }

        /// <summary>
        /// Called before the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnBeforeExecute(IExecutionContext<T> context)
        {
        }

        /// <summary>
        /// Called after the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnAfterExecute(IExecutionContext<T> context)
        {
        }

        private void ProcessExecuteManyResults(ExecutionOptions options, NodeResult aggregateResult)
        {
            aggregateResult.Status = aggregateResult.ChildResults.AggregateNodeResults(options);

            var exceptions = aggregateResult.GetFailExceptions().ToList();
            if (exceptions.Count > 0)
            {
                LogWriter.Info("Child executions returned {0} exceptions.", exceptions.Count);
                aggregateResult.Exception = exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions);
            }

        }

     }
}