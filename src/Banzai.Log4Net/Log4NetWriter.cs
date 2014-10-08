using System;
using Banzai.Logging;
using log4net;

namespace Banzai.Log4Net
{
    /// <summary>
    /// ILogWriter implementation provided by Log4Net.
    /// </summary>
    public class Log4NetWriter : ILogWriter
    {
        private readonly ILog _logger;

        /// <summary>
        /// Constructs a new Log4NetWriter.
        /// </summary>
        /// <param name="type">Type for which the writer will log.</param>
        public Log4NetWriter(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        /// <summary>
        /// Log a fatal error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(string message, Exception exception = null)
        {
            _logger.Fatal(message, exception);
        }

        /// <summary>
        /// Log a fatal exception.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Fatal(string format, params object[] formatArgs)
        {
            _logger.FatalFormat(format, formatArgs);
        }

        /// <summary>
        /// Log a fatal exception.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsFatalEnabled)
            {
                _logger.Fatal(deferredWrite(), exception);
            }
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(string message, Exception exception = null)
        {
            _logger.Error(message, exception);
        }

        /// <summary>
        /// Log an error.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Error(string format, params object[] formatArgs)
        {
            _logger.ErrorFormat(format, formatArgs);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsErrorEnabled)
            {
                _logger.Error(deferredWrite(), exception);
            }
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(string message, Exception exception = null)
        {
            _logger.Warn(message, exception);
        }

        /// <summary>
        /// Log a warning. 
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Warn(string format, params object[] formatArgs)
        {
            _logger.WarnFormat(format, formatArgs);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsWarnEnabled)
            {
                _logger.Warn(deferredWrite(), exception);
            }
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(string message, Exception exception = null)
        {
            _logger.Info(message, exception);
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Info(string format, params object[] formatArgs)
        {
            _logger.InfoFormat(format, formatArgs);
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                _logger.Info(deferredWrite(), exception);
            }
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Debug(string message, Exception exception = null)
        {
            _logger.Debug(message, exception);
        }

        /// <summary>
        /// Log a debug message.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Debug(string format, params object[] formatArgs)
        {
            _logger.DebugFormat(format, formatArgs);
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Debug(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(deferredWrite(), exception);
            }
        }

    }
}