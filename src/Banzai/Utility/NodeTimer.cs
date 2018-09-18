using System.Diagnostics;
using Banzai.Logging;

namespace Banzai.Utility
{
    /// <summary>
    /// Used to time nodes and write to the log.
    /// </summary>
    internal class NodeTimer
    {
        private Stopwatch _stopwatch;

        public string StartTimer(dynamic instance, string methodName)
        {
            _stopwatch = Stopwatch.StartNew();
            return $"Starting stopwatch for methodName {methodName} of class {instance.GetType().FullName} with nodeId {instance.Id} from flowId {instance.FlowId}.";
        }

        public string StopTimer(dynamic instance, string methodName)
        {
            if (_stopwatch != null && _stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                return $"Stopping stopwatch for methodName {methodName} of class {instance.GetType().FullName} with nodeId {instance.Id} from flowId {instance.FlowId}. Elapsed ms: {elapsed}";
            }
            return $"Call to stop occurred, but stopwatch not started. Class {instance.GetType().FullName}, Method {methodName}, NodeId {instance.Id}, FlowId {instance.FlowId}. ";
        }

        public void LogStart(ILogWriter logWriter, dynamic node, string methodName)
        {
            logWriter.Debug(() => StartTimer(node, methodName));
        }

        public void LogStop(ILogWriter logWriter, dynamic node, string methodName)
        {
            logWriter.Debug(() => StopTimer(node, methodName));
        }
    }
}