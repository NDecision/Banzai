using System;
using Banzai.Logging;

namespace Banzai.Log4Net
{
    public class Log4NetWriterFactory : ILogWriterFactory
    {
        public ILogWriter GetLogger(Type type)
        {
            return new Log4NetWriter(type);
        }
    }
}