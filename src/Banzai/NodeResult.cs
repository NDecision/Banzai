using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Banzai
{
    /// <summary>
    /// The results of a node execution.
    /// </summary>
    public sealed class NodeResult
    {
        private readonly ConcurrentQueue<NodeResult> _childResults = new ConcurrentQueue<NodeResult>();

        /// <summary>
        /// Constructs a new NodeResult.
        /// </summary>
        /// <param name="subject">Subject operated on by the node.</param>
        /// <param name="id">The id of the node that produced this result.</param>
        /// <param name="flowId">The id of the flow that contains this node.</param>
        public NodeResult(object subject, string id, string flowId)
        {
            Subject = subject;
            Id = id;
            FlowId = flowId;
        }

        /// <summary>
        /// Id of the current node, can be set to help with debugging.  Defaults to the node type name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the current flow, can be set to help with debugging.
        /// </summary>
        public string FlowId { get; set; }
        
        /// <summary>
        /// Subject operated on by the node.
        /// </summary>
        public object Subject { get; internal set; }

        /// <summary>
        /// Convenience method to allow the subject to be retrieved and casted at teh same time.
        /// </summary>
        /// <typeparam name="T">Type to cast the subject to.</typeparam>
        /// <returns>Typed subject.</returns>
        public T GetSubjectAs<T>()
        {
            return (T) Subject;
        }

        /// <summary>
        /// Success status of the node operation.
        /// </summary>
        public NodeResultStatus Status { get; internal set; }

        /// <summary>
        /// Exception, if any, that happened on this node during execution.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Child results if the node contained child nodes.
        /// </summary>
        public IEnumerable<NodeResult> ChildResults => _childResults;

        /// <summary>
        /// If the node was a failure, aggregates execptions along the failure path.
        /// </summary>
        /// <remarks>
        /// This does not include all exceptions.  Exceptions that are descendents of nodes that are not marked as failures are not included.
        /// The basic idea is that these exceptions were important in the failure of the inspected node.
        /// </remarks>
        public IEnumerable<Exception> GetFailExceptions()
        {
            var exceptions = new List<Exception>();

            AppendFailExceptions(exceptions);

            return exceptions;
        }

        /// <summary>
        /// Appends exceptions from this node to the requested collection and recurses children.
        /// </summary>
        /// <param name="exceptions">Exceptions list to add items to.</param>
        private void AppendFailExceptions(ICollection<Exception> exceptions)
        {
            if (Status == NodeResultStatus.Failed)
            {
                if (Exception != null)
                {
                    exceptions.Add(Exception);
                }

                foreach (var childResult in _childResults)
                {
                    childResult.AppendFailExceptions(exceptions);
                }
            }
        }

        /// <summary>
        /// Adds a child result to the current result.
        /// </summary>
        /// <param name="result">Result to add to child results.</param>
        internal void AddChildResult(NodeResult result)
        {
            _childResults.Enqueue(result);
        }

        /// <summary>
        /// Adds a child result to the current result.
        /// </summary>
        /// <param name="results">Results to add to child results.</param>
        internal void AddChildResults(IEnumerable<NodeResult> results)
        {
            foreach (var result in results)
            {
                _childResults.Enqueue(result);
            }
        }

    }
}