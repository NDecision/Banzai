using System;
using System.Collections.Concurrent;
using Banzai.Utility;

namespace Banzai.Logging
{
    /// <summary>
    /// Factory implementation for retrieving a log writer.
    /// </summary>
    public static class LogWriter
    {
        private static ILogWriterFactory _factory = new DebugLogWriterFactory();
        private static readonly ConcurrentDictionary<Type, ILogWriter> _writerCache = new ConcurrentDictionary<Type, ILogWriter>();

        /// <summary>
        /// Sets the internal factory to the desired ILogWriterFactory
        /// </summary>
        /// <param name="factory">Factory to use for getting log writers.</param>
        public static void SetFactory(ILogWriterFactory factory)
        {
            Guard.AgainstNullArgument("factory", factory);

            _factory = factory;
            _writerCache.Clear();
        }

        /// <summary>
        /// Method for getting a log writer from the current factory.
        /// </summary>
        /// <typeparam name="T">Type for which to get a logger.</typeparam>
        /// <param name="instance">Instance from whose type the logger is created.</param>
        /// <returns>An ILogWriter.</returns>
        public static ILogWriter GetLogger<T>(T instance)
        {
            var type = instance.GetType();
            ILogWriter writer = _writerCache.GetOrAdd(type, _factory.GetLogger(type));

            return writer;
        }
    }
}