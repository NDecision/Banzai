using System;
using System.Linq;
using System.Reflection;

namespace Banzai.Logging
{
    /// <summary>
    /// Default logger that writes to the debug output.
    /// </summary>
    public class DebugLogWriter : ILogWriter
    {
        private readonly string _typeName;

        /// <summary>
        /// Constructs a new DebugLogWriter.
        /// </summary>
        /// <param name="type">Type for which logger is created.</param>
        public DebugLogWriter(Type type)
        {
            _typeName = type.Name;

            Type[] genericArgs = type.GetTypeInfo().GetGenericArguments();
            if (genericArgs.Length > 0)
            {
                string argsString = string.Join(", ", genericArgs.Select(x => x.Name));
                _typeName += "[" + argsString + "]";
            }
        }

        /// <summary>
        /// Log a fatal error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + message, exception));
        }

        /// <summary>
        /// Log a fatal exception.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be messageformatted into the message format.</param>
        public void Fatal(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + string.Format(format, formatArgs)));
        }

        /// <summary>
        /// Log a fatal exception.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Fatal: " + deferredWrite(), exception));
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + message, exception));
        }

        /// <summary>
        /// Log an error.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be messageformatted into the message format.</param>
        public void Error(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + string.Format(format, formatArgs)));
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Error: " + deferredWrite(), exception));
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + message, exception));
        }

        /// <summary>
        /// Log a warning. 
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Warn(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + string.Format(format, formatArgs)));
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Warn: " + deferredWrite(), exception));
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + message, exception));
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Info(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + string.Format(format, formatArgs)));
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(Func<string> deferredWrite, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Info: " + deferredWrite(), exception));
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Debug(string message, Exception exception = null)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Debug: " + message, exception));
        }

        /// <summary>
        /// Log a debug message.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Debug(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(BuildMessage("Debug: " + string.Format(format, formatArgs)));
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
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