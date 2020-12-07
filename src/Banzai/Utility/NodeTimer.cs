using System.Diagnostics;
using Microsoft.Extensions.Logging;

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
            return $"Starting stopwatch for methodName {methodName} of class {instance.GetType().FullName} with " +
                   $"nodeId {instance.Id} from flowId {instance.FlowId}.";
        }

        public string StopTimer(dynamic instance, string methodName)
        {
            if (_stopwatch != null && _stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                return $"Stopping stopwatch for methodName {methodName} of class {instance.GetType().FullName} with " +
                       $"nodeId {instance.Id} from flowId {instance.FlowId}. Elapsed ms: {elapsed}";
            }

            return $"Call to stop occurred, but stopwatch not started. Class {instance.GetType().FullName}, " +
                   $"Method {methodName}, NodeId {instance.Id}, FlowId {instance.FlowId}. ";
        }

        public void LogStart(ILogger logger, dynamic node, string methodName)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                string message = StartTimer(node, methodName);
                logger.LogDebug(message);
            }
        }

        public void LogStop(ILogger logger, dynamic node, string methodName)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                string message = StopTimer(node, methodName);
                logger.LogDebug(message);
            }
        }
    }
}