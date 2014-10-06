using System;
using System.Linq;

namespace Banzai.Logging
{
    /// <summary>
    /// Default logger that writes to the debug output.
    /// </summary>
    public class DebugLogWriter : ILogWriter
    {
        private readonly string _typeName;

        public DebugLogWriter(Type type)
        {
            _typeName = type.Name;

            Type[] genericArgs = type.GetGenericArguments();
            if (genericArgs.Length > 0)
            {
                string argsString = string.Join(", ", genericArgs.Select(x => x.Name));
                _typeName += "[" + argsString + "]";
            }
        }

        public void Fatal(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + message, exception));
        }

        public void Fatal(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + string.Format(format, formatArgs)));
        }

        public void Fatal(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + deferredWrite(), exception));
        }

        public void Error(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + message, exception));
        }

        public void Error(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + string.Format(format, formatArgs)));
        }

        public void Error(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + deferredWrite(), exception));
        }

        public void Warn(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + message, exception));
        }

        public void Warn(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + string.Format(format, formatArgs)));
        }

        public void Warn(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + deferredWrite(), exception));
        }

        public void Info(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + message, exception));
        }

        public void Info(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + string.Format(format, formatArgs)));
        }

        public void Info(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + deferredWrite(), exception));
        }

        public void Debug(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Debug: " + message, exception));
        }

        public void Debug(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Debug: " + string.Format(format, formatArgs)));
        }

        public void Debug(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Debug: " + deferredWrite(), exception));
        }


        private string BuildMessage(string message, Exception exception = null)
        {
            var fullMessage = DateTime.UtcNow.ToString("[yyyy-MM-dd hh:mm:ss] - ") + _typeName + " - Message: " + message 
                + (exception != null ? "\nException: " + exception : null);
            return fullMessage;
        }

    }
}