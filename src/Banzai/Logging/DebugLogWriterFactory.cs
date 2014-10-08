using System;

namespace Banzai.Logging
{
    /// <summary>
    /// Factory for providing a DebugLogWriter.
    /// </summary>
    public class DebugLogWriterFactory : ILogWriterFactory
    {
        /// <summary>
        /// Gets a log writer based on the passed type.
        /// </summary>
        /// <param name="type">Type for which to get a log writer.</param>
        /// <returns>An ILogWriter</returns>
        public ILogWriter GetLogger(Type type)
        {
            return new DebugLogWriter(type);
        }
    }
}