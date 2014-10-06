using System;
using System.Collections.Concurrent;

namespace Banzai.Logging
{
    public static class LogWriter
    {
        private static ILogWriterFactory _factory;
        private static readonly ConcurrentDictionary<Type, ILogWriter> _writerCache = new ConcurrentDictionary<Type, ILogWriter>();

        public static void SetFactory(ILogWriterFactory factory)
        {
            _factory = factory;
        }

        public static ILogWriter GetLogger<T>(T instance)
        {
            var type = typeof (T);
            ILogWriter writer = _writerCache.GetOrAdd(type, _factory.GetLogger(type));

            return writer;
        }
    }
}