using System;

namespace Banzai.Logging
{
    public class DebugLogWriterFactory : ILogWriterFactory
    {
        public ILogWriter GetLogger(Type type)
        {
            return new DebugLogWriter(type);
        }
    }
}