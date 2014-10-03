using System;
using System.Globalization;

namespace Banzai.Utility
{
    /// <summary>
    /// Provides guard clauses.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Guards against a null argument.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="property">The argument.</param>
        /// <exception cref="System.NullReferenceException"><paramref name="property" /> is <c>null</c>.</exception>
        public static void AgainstNullProperty<TProperty>(string propertyName, TProperty property) where TProperty : class
        {
            if (property == null)
            {
                throw new NullReferenceException(string.Format(CultureInfo.InvariantCulture, "{0} is null.", propertyName));
            }
        }

        /// <summary>
        /// Guards against a null argument.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="argument">The argument.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="argument" /> is <c>null</c>.</exception>
        public static void AgainstNullArgument<TArgument>(string parameterName, TArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.InvariantCulture, "{0} is null.", parameterName));
            }
        }

        /// <summary>
        /// Guards against a null argument property value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="argumentProperty">The argument property.</param>
        /// <exception cref="System.ArgumentException"><paramref name="argumentProperty" /> is <c>null</c>.</exception>
        public static void AgainstNullArgumentProperty<TProperty>(string parameterName, string propertyName, TProperty argumentProperty)
        {
            if (argumentProperty == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0}.{1} is null.", parameterName, propertyName), parameterName);
            }
        }

        /// <summary>
        /// Guards against a null or empty argument.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="argument">The argument.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="argument" /> is <c>null</c>.</exception>
        public static void AgainstNullOrEmptyArgument(string parameterName, string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.InvariantCulture, "{0} is null or empty.", parameterName));
            }
        }

    }
}
