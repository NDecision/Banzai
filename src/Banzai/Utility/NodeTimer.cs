using System;
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

        public string StartTimer(string typeName, string methodName)
        {
            _stopwatch = Stopwatch.StartNew();
            return string.Format("Starting stopwatch for methodName {0} of class {1}.", methodName, typeName);
        }

        public string StopTimer(string typeName, string methodName)
        {
            if (_stopwatch != null && _stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                return string.Format("Stopping stopwatch for methodName {0} of class {1}. Elapsed ms: {2}", methodName, typeName, elapsed);
            }
            return string.Format("Call to stop occurred, but stopwatch not started. Class {0}, Method {1}. ", typeName, methodName);
        }

        public void LogStart(ILogWriter logWriter, object instance, string methodName)
        {
            logWriter.Debug(() => StartTimer(instance.GetType().FullName, methodName));
        }

        public void LogStop(ILogWriter logWriter, object instance, string methodName)
        {
            logWriter.Debug(() => StopTimer(instance.GetType().FullName, methodName));
        }
    }
}