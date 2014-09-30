using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Banzai.Core
{
    public class NodeResult<T>
    {
        private readonly ConcurrentQueue<NodeResult<T>> _childResults = new ConcurrentQueue<NodeResult<T>>();

        public NodeResult(T subject)
        {
            Subject = subject;
        }
        
        public T Subject { get; private set; }

        public NodeResultStatus Status { get; protected internal set; }

        public Exception Exception { get; protected internal set; }

        public IEnumerable<NodeResult<T>> ChildResults { get { return _childResults; } }

        public void AddChildResult(NodeResult<T> result)
        {
            _childResults.Enqueue(result);
        }

    }
}