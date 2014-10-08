using System;
using Banzai.Logging;

namespace Banzai.Log4Net
{
    /// <summary>
    /// Log4Net implementation of ILogWriterFactory.  Used to provide a Log4Net ILogWriter.
    /// </summary>
    public class Log4NetWriterFactory : ILogWriterFactory
    {
        /// <summary>
        /// Gets a log writer based on the passed type.
        /// </summary>
        /// <param name="type">Type for which to get a log writer.</param>
        /// <returns>An ILogWriter</returns>
        public ILogWriter GetLogger(Type type)
        {
            return new Log4NetWriter(type);
        }
    }
}