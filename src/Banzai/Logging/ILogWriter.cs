using System;

namespace Banzai.Logging
{
    /// <summary>
    /// Interface for the LogWriter exposed in an INode.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Log a fatal error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Fatal(string message, Exception exception = null);

        /// <summary>
        /// Log a fatal exception.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        void Fatal(string format, params object[] formatArgs);

        /// <summary>
        /// Log a fatal exception.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Fatal(Func<string> deferredWrite, Exception exception = null);

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Error(string message, Exception exception = null);

        /// <summary>
        /// Log an error.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        void Error(string format, params object[] formatArgs);

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Error(Func<string> deferredWrite, Exception exception = null);

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Warn(string message, Exception exception = null);

        /// <summary>
        /// Log a warning. 
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        void Warn(string format, params object[] formatArgs);

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Warn(Func<string> deferredWrite, Exception exception = null);

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Info(string message, Exception exception = null);

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        void Info(string format, params object[] formatArgs);

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Info(Func<string> deferredWrite, Exception exception = null);

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Debug(string message, Exception exception = null);

        /// <summary>
        /// Log a debug message.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        void Debug(string format, params object[] formatArgs);

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        void Debug(Func<string> deferredWrite, Exception exception = null);

    }
}