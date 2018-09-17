using System;
using Banzai.Logging;

namespace Banzai.Serilog
{
    /// <summary>
    /// Serilog implementation of ILogWriterFactory.  Used to provide a Serilog ILogWriter.
    /// </summary>
    public class SerilogWriterFactory : ILogWriterFactory
    {
        /// <summary>
        /// Gets a log writer based on the passed type.
        /// </summary>
        /// <param name="type">Type for which to get a log writer.</param>
        /// <returns>An ILogWriter</returns>
        public ILogWriter GetLogger(Type type)
        {
            return new SerilogWriter(type);
        }
    }
}