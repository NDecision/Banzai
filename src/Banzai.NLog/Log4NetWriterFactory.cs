using System;
using Banzai.Logging;

namespace Banzai.NLog
{
    /// <summary>
    /// NLog implementation of ILogWriterFactory.  Used to provide a NLog ILogWriter.
    /// </summary>
    public class NLogWriterFactory : ILogWriterFactory
    {
        /// <summary>
        /// Gets a log writer based on the passed type.
        /// </summary>
        /// <param name="type">Type for which to get a log writer.</param>
        /// <returns>An ILogWriter</returns>
        public ILogWriter GetLogger(Type type)
        {
            return new NLogWriter(type);
        }
    }
}