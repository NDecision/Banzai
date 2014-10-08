using System;
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
        bool ShouldExecute(ExecutionContext<T> context);

        /// <summary>
        /// Function to define if this node should be executed.
        /// </summary>
        Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Method that defines the async function to call to determine if this node should be executed.
        /// </summary>
        Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

        /// <summary>
        /// Function to be performed when the node is executed.
        /// </summary>
        Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Method that defines the async function to execute on the subject for this node.
        /// </summary>
        Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecutedFuncAsync { get; set; }

        /// <summary>
        /// Determines if the node should be executed.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>Bool indicating if the current node should be run.</returns>
        Task<bool> ShouldExecuteAsync(ExecutionContext<T> context);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult<T>> ExecuteAsync(T subject);

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="sourceContext">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> sourceContext);


        /// <summary>
        /// Used to reset the node to a prerun state
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// The basic class for a functional node to be run by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that the pipeline acts upon.</typeparam>
    public class Node<T> : INode<T>
    {
        /// <summary>
        /// Creates a new Node.
        /// </summary>
        public Node()
        {
            ShouldExecuteFunc = ShouldExecute;
            ExecutedFunc = PerformExecute;

            ShouldExecuteFuncAsync = ShouldExecuteAsync;
            ExecutedFuncAsync = PerformExecuteAsync;
        }

        /// <summary>
        /// Creates a new Node with local options to override global options.
        /// </summary>
        /// <param name="localOptions">Local options to override global options.</param>
        public Node(ExecutionOptions localOptions) : this()
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
        /// Synchronous function to determine if node ShouldExecute.  Takes precedence above overridden ShouldExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, bool> ShouldExecuteFunc { get; set; }

        /// <summary>
        /// Function used to evaluate if this node should execute.  Takes precedence over overridden ShouldExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

        /// <summary>
        /// Synchronous function that provides functionality for the node.  Takes precedence above overridden PerformExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, NodeResultStatus> ExecutedFunc { get; set; }

        /// <summary>
        /// Function executed when the node executes. Takes precedence over overridden PerformExecute method.
        /// </summary>
        public Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecutedFuncAsync { get; set; }

        /// <summary>
        /// Determines if the current node should execute with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual bool ShouldExecute(ExecutionContext<T> context)
        {
            return true;
        }

        /// <summary>
        /// Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual Task<bool> ShouldExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ShouldExecuteFunc(context));
        }

        /// <summary>
        /// Used to kick off execution of a node with a default execution context.
        /// </summary>
        /// <param name="subject">Subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult<T>> ExecuteAsync(T subject)
        {
            return await ExecuteAsync(new ExecutionContext<T>(subject));
        }

        /// <summary>
        /// Used to kick off execution of a node with a specified execution context.
        /// </summary>
        /// <param name="sourceContext">ExecutionContext that includes a subject to be moved through the node.</param>
        /// <returns>A NodeResult</returns>
        public async Task<NodeResult<T>> ExecuteAsync(ExecutionContext<T> sourceContext)
        {
            Guard.AgainstNullArgument("context", sourceContext);
            Guard.AgainstNullArgumentProperty("context", "Subject", sourceContext.Subject);

            if (Status != NodeRunStatus.NotRun)
            {
                LogWriter.Debug("Status does not equal 'NotRun', resetting the node before execution");
                Reset();
            }

            var subject = sourceContext.Subject;
            var result = new NodeResult<T>(subject);

            ExecutionContext<T> context = PrepareExecutionContext(sourceContext, result);

            OnBeforeExecute(context);

            if (! await ShouldExecuteFuncAsync(context).ConfigureAwait(false))
            {
                LogWriter.Info("ShouldExecute returned a false, skipping execution");
                return result;
            }

            Status = NodeRunStatus.Running;
            LogWriter.Debug("Executing the node");

            try
            {
                result.Status = await ExecutedFuncAsync(context).ConfigureAwait(false);
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

                if (sourceContext.EffectiveOptions.ThrowOnError)
                {
                    throw;
                }
            }

            OnAfterExecute(context);

            return result;
        }

        /// <summary>
        /// Method to override to provide functionality to the current node with synchronous wrapper.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual NodeResultStatus PerformExecute(ExecutionContext<T> context)
        {
            return NodeResultStatus.Succeeded;
        }

        /// <summary>
        /// Method to override to provide functionality to the current node.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Final result execution status of the node.</returns>
        protected virtual Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(ExecutedFunc(context));
        }

        /// <summary>
        /// Prepares the execution context before the current node is run.
        /// </summary>
        /// <param name="context">Source context for preparation.</param>
        /// <param name="currentResult">A referene to the result of the current node.</param>
        /// <returns>The execution context to be used in node execution.</returns>
        protected virtual ExecutionContext<T> PrepareExecutionContext(ExecutionContext<T> context, NodeResult<T> currentResult)
        {
            LogWriter.Debug("Preparing the execution context for execution.");
            context.AddResult(currentResult);

            if (LocalOptions != null)
            {
                LogWriter.Debug("Local options detected, merging with global settings. Local Options - ContinueOnFailure:{0}", 
                    LocalOptions.ContinueOnFailure, LocalOptions.ThrowOnError);
                context.EffectiveOptions = LocalOptions;
            }
            return context;
        }

        /// <summary>
        /// Resets the current node to unrun state.
        /// </summary>
        public virtual void Reset()
        {
            Status = NodeRunStatus.NotRun;
        }

        /// <summary>
        /// Called before the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnBeforeExecute(ExecutionContext<T> context)
        {
        }

        /// <summary>
        /// Called after the node is executed. Override to add functionality.
        /// </summary>
        /// <param name="context">Effective context for execution.</param>
        protected virtual void OnAfterExecute(ExecutionContext<T> context)
        {
        }

     }
}