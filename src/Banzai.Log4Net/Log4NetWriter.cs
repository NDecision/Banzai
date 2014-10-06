using System;
using Banzai.Logging;
using log4net;

namespace Banzai.Log4Net
{
    public class Log4NetWriter : ILogWriter
    {
        private readonly ILog _logger;

        public Log4NetWriter(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        public void Fatal(string message, Exception exception = null)
        {
            _logger.Fatal(message, exception);
        }

        public void Fatal(string messageFormat, params object[] formatArgs)
        {
            _logger.FatalFormat(messageFormat, formatArgs);
        }

        public void Fatal(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsFatalEnabled)
            {
                _logger.Fatal(deferredWrite(), exception);
            }
        }

        public void Error(string message, Exception exception = null)
        {
            _logger.Error(message, exception);
        }

        public void Error(string messageFormat, params object[] formatArgs)
        {
            _logger.ErrorFormat(messageFormat, formatArgs);
        }

        public void Error(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsErrorEnabled)
            {
                _logger.Error(deferredWrite(), exception);
            }
        }

        public void Warn(string message, Exception exception = null)
        {
            _logger.Warn(message, exception);
        }

        public void Warn(string messageFormat, params object[] formatArgs)
        {
            _logger.WarnFormat(messageFormat, formatArgs);
        }

        public void Warn(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsWarnEnabled)
            {
                _logger.Warn(deferredWrite(), exception);
            }
        }

        public void Info(string message, Exception exception = null)
        {
            _logger.Info(message, exception);
        }

        public void Info(string messageFormat, params object[] formatArgs)
        {
            _logger.InfoFormat(messageFormat, formatArgs);
        }

        public void Info(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsInfoEnabled)
            {
                _logger.Info(deferredWrite(), exception);
            }
        }

        public void Debug(string message, Exception exception = null)
        {
            _logger.Debug(message, exception);
        }

        public void Debug(string messageFormat, params object[] formatArgs)
        {
            _logger.DebugFormat(messageFormat, formatArgs);
        }

        public void Debug(Func<string> deferredWrite, Exception exception = null)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug(deferredWrite(), exception);
            }
        }

    }
}