using System;
using System.Collections.Concurrent;
using Banzai.Utility;

namespace Banzai.Logging
{
    public static class LogWriter
    {
        private static ILogWriterFactory _factory = new DebugLogWriterFactory();
        private static readonly ConcurrentDictionary<Type, ILogWriter> _writerCache = new ConcurrentDictionary<Type, ILogWriter>();

        public static void SetFactory(ILogWriterFactory factory)
        {
            Guard.AgainstNullArgument("factory", factory);

            _factory = factory;
        }

        public static ILogWriter GetLogger<T>(T instance)
        {
            var type = instance.GetType();
            ILogWriter writer = _writerCache.GetOrAdd(type, _factory.GetLogger(type));

            return writer;
        }
    }
}