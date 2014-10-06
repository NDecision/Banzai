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
        /// Method that defines the async function to call to determine if this node should be executed.
        /// </summary>
        Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

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


    public class Node<T> : INode<T>
    {
        public Node()
        {
            ShouldExecuteFuncAsync = ShouldExecuteAsync;
            ExecutedFuncAsync = PerformExecuteAsync;
        }

        public Node(ExecutionOptions localOptions) : this()
        {
            LocalOptions = localOptions;
        }
        
        public ExecutionOptions LocalOptions { get; set; }

        public ILogWriter LogWriter { get { return Logging.LogWriter.GetLogger(this); } }

        public Func<ExecutionContext<T>, Task<bool>> ShouldExecuteFuncAsync { get; set; }

        public Func<ExecutionContext<T>, Task<NodeResultStatus>> ExecutedFuncAsync { get; set; }

        /// <summary>
        /// Determines if the current node should execute.
        /// </summary>
        /// <param name="context">Current ExecutionContext</param>
        /// <returns>Bool indicating if this node should run.</returns>
        public virtual async Task<bool> ShouldExecuteAsync(ExecutionContext<T> context)
        {
            return true;
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

            if (! await ShouldExecuteFuncAsync(context))
            {
                LogWriter.Info("ShouldExecute returned a false, skipping execution");
                return result;
            }

            Status = NodeRunStatus.Running;
            LogWriter.Debug("Executing the node");

            try
            {
                result.Status = await ExecutedFuncAsync(context);
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
            return result;
        }

        protected virtual Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<T> context)
        {
            return Task.FromResult(NodeResultStatus.Succeeded);
        }

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

        public virtual void Reset()
        {
            Status = NodeRunStatus.NotRun;
        }

        public NodeRunStatus Status { get; private set; }
     }
}