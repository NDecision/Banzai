using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Banzai.Serialization
{
    /// <summary>
    /// Provides a cache for node type naming used in serialization.
    /// </summary>
    public static class TypeAbbreviationCache
    {
        private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<Type, string> _nameCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Register core node types.
        /// </summary>
        public static void RegisterCoreTypes()
        {
            RegisterType(typeof(Object));
            //AutoRegister the canned node types
            RegisterFromAssembly(typeof(INode<>).Assembly);
        }

        /// <summary>
        /// Register a specific type.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="name">Key to use (overrides the type name)</param>
        public static void RegisterType(Type type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = type.Name;

            _typeCache.TryAdd(name, type);
            _nameCache.TryAdd(type, name);
        }

        /// <summary>
        /// Register node types from the provided assembly.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        public static void RegisterFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(INode<>)));
            
            foreach (var type in types)
            {
                RegisterType(type);
            }
        }

        /// <summary>
        /// Try to get the type from the abbreviation cache
        /// </summary>
        /// <param name="name">Key to look for.</param>
        /// <param name="type">Type to return.</param>
        /// <returns></returns>
        public static bool TryGetType(string name, out Type type)
        {
            return _typeCache.TryGetValue(name, out type);
        }

        /// <summary>
        /// Try to get the type from the abbreviation cache
        /// </summary>
        /// <param name="name">Name to return.</param>
        /// <param name="type">Type to look for.</param>
        /// <returns></returns>
        public static bool TryGetName(Type type, out string name)
        {
            return _nameCache.TryGetValue(type, out name);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void Clear()
        {
            _nameCache.Clear();
            _typeCache.Clear();
        }
    }
}