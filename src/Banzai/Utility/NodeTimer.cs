using System.Diagnostics;
using Banzai.Logging;

namespace Banzai.Utility
{
    /// <summary>
    /// Used to time nodes and write to the log.
    /// </summary>
    internal class NodeTimer<T>
    {
        private Stopwatch _stopwatch;

        public string StartTimer(INode<T> node, string methodName)
        {
            _stopwatch = Stopwatch.StartNew();
            return string.Format("Starting stopwatch for methodName {0} of class {1} with nodeId {2} from flowId {3}.", methodName, node.GetType().FullName, node.Id, node.FlowId);
        }

        public string StopTimer(INode<T> node, string methodName)
        {
            if (_stopwatch != null && _stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                return string.Format("Stopping stopwatch for methodName {0} of class {1} with nodeId {2} from flowId {3}. Elapsed ms: {4}", methodName, node.GetType().FullName, node.Id, node.FlowId, elapsed);
            }
            return string.Format("Call to stop occurred, but stopwatch not started. Class {0}, Method {1}, NodeId {2}, FlowId {3}. ", node.GetType().FullName, methodName, node.Id, node.FlowId);
        }

        public void LogStart(ILogWriter logWriter, INode<T> node, string methodName)
        {
            logWriter.Debug(() => StartTimer(node, methodName));
        }

        public void LogStop(ILogWriter logWriter, INode<T> node, string methodName)
        {
            logWriter.Debug(() => StopTimer(node, methodName));
        }
    }
}