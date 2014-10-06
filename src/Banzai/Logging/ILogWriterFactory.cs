using System;

namespace Banzai.Logging
{
    public interface ILogWriterFactory
    {
        /// <summary>
        /// Gets a log writer based on the passed type.
        /// </summary>
        /// <param name="type">Type for which to get a log writer.</param>
        /// <returns>An ILogWriter</returns>
        ILogWriter GetLogger(Type type);
    }

}