﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Banzai.Factories;
using Microsoft.Extensions.Logging;

namespace Banzai
{
    /// <summary>
    /// Interface for a Node that allows a transition to another node type.
    /// </summary>
    /// <typeparam name="TSource">Original node type.</typeparam>
    /// <typeparam name="TDestination">Resultant node type.</typeparam>
    public abstract class TransitionNode<TSource, TDestination> : Node<TSource>, ITransitionNode<TSource, TDestination>
    {
        /// <summary>
        /// Gets or sets an injected NodeFactory to use when constructing this node.
        /// </summary>
        public INodeFactory<TDestination> NodeFactory { get; set; }

        /// <summary>
        /// Gets or sets the TDestination child node to operate on.
        /// </summary>
        public INode<TDestination> ChildNode { get; set; }

        /// <summary>
        /// Resets this node and all its children to an unrun state.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ChildNode?.Reset();
        }

        /// <summary>
        /// Executes child nodes of the current node.
        /// </summary>
        /// <param name="context">Current ExecutionContext.</param>
        /// <returns>NodeResultStatus representing the current node result.</returns>
        protected sealed override async Task<NodeResultStatus> PerformExecuteAsync(IExecutionContext<TSource> context)
        {
            if (ChildNode == null)
            {
                Logger.LogWarning("Child node of TransitionNode doesn't exist, node will be skipped.");
                return NodeResultStatus.NotRun;
            }

            Logger.LogDebug("Creating the TransitionNode destination subject.");
            TDestination destSubject = await TransitionSourceAsync(context).ConfigureAwait(false);

            var destContext = new ExecutionContext<TDestination>(destSubject, context.GlobalOptions);

            Logger.LogDebug("Preparing to execute TransitionNode child.");
            NodeResult destResult = await ChildNode.ExecuteAsync(destContext).ConfigureAwait(false);

            var exceptions = destResult.GetFailExceptions().ToList();
            if (exceptions.Count > 0)
            {
                Logger.LogInformation("TransitionNode child returned {0} exceptions.", exceptions.Count);
                context.ParentResult.Exception = exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions);
            }
            Logger.LogDebug("Creating the TransitionNode destination result.");
            var resultSubject = await TransitionResultAsync(context, destResult).ConfigureAwait(false);
            
            if (!context.Subject.Equals(resultSubject))
            {
                Logger.LogDebug("Source subject has changed, calling ChangeSubject.");
                context.ChangeSubject(resultSubject);
            }

            return destResult.Status;
        }

        /// <summary>
        /// Transitions from the source subject to the destination subject.
        /// </summary>
        /// <param name="sourceContext">The source execution context, including the subject.</param>
        /// <returns></returns>
        protected virtual Task<TDestination> TransitionSourceAsync(IExecutionContext<TSource> sourceContext)
        {
            return Task.FromResult(default(TDestination));
        }

        /// <summary>
        /// Transitions the source based on the child result to prepare for. 
        /// </summary>
        /// <param name="sourceContext">Context including the source subject.</param>
        /// <param name="result">The result of the destination node.</param>
        /// <returns>The transitioned subject.</returns>
        protected virtual Task<TSource> TransitionResultAsync(IExecutionContext<TSource> sourceContext, NodeResult result)
        {
            return Task.FromResult(sourceContext.Subject);
        }

    }
}