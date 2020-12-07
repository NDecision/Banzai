using Microsoft.Extensions.Logging;

namespace Banzai.Logging
{
    /// <summary>
    ///     Provides loggers via the static LoggerFactory to the rest of the application
    /// </summary>
    public static class LoggerProvider
    {
        private static readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        /// <summary>
        ///     Creates a logger for the given type.
        /// </summary>
        /// <typeparam name="T">The type for which to a logger is created.</typeparam>
        /// <returns>The requested logger.</returns>
        public static ILogger CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        ///     Creates a logger for the given instance type.
        /// </summary>
        /// <typeparam name="T">The type for which to a logger is created.</typeparam>
        /// <param name="instance">The instance used to derive a type for which a logger is created.</param>
        /// <returns>The requested logger.</returns>
        public static ILogger CreateLogger<T>(T instance)
        {
            return _loggerFactory.CreateLogger(instance.GetType());
        }
    }
}