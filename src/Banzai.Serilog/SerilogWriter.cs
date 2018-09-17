using System;
using Banzai.Logging;
using Serilog;
using Serilog.Events;

namespace Banzai.Serilog
{
    /// <summary>
    /// ILogWriter implementation provided by Serilog.
    /// </summary>
    public class SerilogWriter : ILogWriter
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new SerilogWriter.
        /// </summary>
        /// <param name="type">Type for which the writer will log.</param>
        public SerilogWriter(Type type)
        {
            _logger = Log.Logger.ForContext(type);
        }

        /// <summary>
        /// Log a fatal error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(string message, Exception exception = null)
        {
            _logger.Fatal(exception, message);
        }

        /// <summary>
        /// Log a fatal exception.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Fatal(string format, params object[] formatArgs)
        {
            _logger.Fatal(format, formatArgs);
        }

        /// <summary>
        /// Log a fatal exception.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Fatal(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Fatal))
            {
                _logger.Fatal(exception, deferredWrite());
            }
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(string message, Exception exception = null)
        {
            _logger.Error(exception, message);
        }

        /// <summary>
        /// Log an error.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Error(string format, params object[] formatArgs)
        {
            _logger.Error(format, formatArgs);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Error(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Error))
            {
                _logger.Error(exception, deferredWrite());
            }
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(string message, Exception exception = null)
        {
            _logger.Warning(exception, message);
        }

        /// <summary>
        /// Log a warning. 
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Warn(string format, params object[] formatArgs)
        {
            _logger.Warning(format, formatArgs);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Warn(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Warning))
            {
                _logger.Warning(exception, deferredWrite());
            }
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(string message, Exception exception = null)
        {
            _logger.Information(exception, message);
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Info(string format, params object[] formatArgs)
        {
            _logger.Information(format, formatArgs);
        }

        /// <summary>
        /// Log an info message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Info(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Information))
            {
                _logger.Information(exception, deferredWrite());
            }
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Debug(string message, Exception exception = null)
        {
            _logger.Debug(exception, message);
        }

        /// <summary>
        /// Log a debug message.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Debug(string format, params object[] formatArgs)
        {
            _logger.Debug(format, formatArgs);
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Debug(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug(exception, deferredWrite());
            }
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Trace(string message, Exception exception = null)
        {
            _logger.Verbose(exception, message);
        }

        /// <summary>
        /// Log a debug message.  
        /// </summary>
        /// <param name="format">Format of the message.  Defers format until logging level is assessed.</param>
        /// <param name="formatArgs">Args to be formatted into the message format.</param>
        public void Trace(string format, params object[] formatArgs)
        {
            _logger.Verbose(format, formatArgs);
        }

        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="deferredWrite">Defers the write operation until logging level is assessed. 
        /// Can be useful when expensive concatenation operations are required.</param>
        /// <param name="exception">Exception associated with the error.</param>
        public void Trace(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsEnabled(LogEventLevel.Verbose))
            {
                _logger.Verbose(exception, deferredWrite());
            }
        }

    }
}