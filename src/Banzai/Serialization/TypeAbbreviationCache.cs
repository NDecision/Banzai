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
            RegisterType(typeof(Object), failOnCollision:false);
            //AutoRegister the canned node types
            RegisterFromAssembly(typeof(INode<>).Assembly, failOnCollision:false);
        }

        /// <summary>
        /// Register a specific type.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="name">Key to use (overrides the type name)</param>
        /// <param name="useFullName">Uses the full name when registering so that it doesn't collide with an existing type name if needed.</param>
        /// <param name="failOnCollision">Fail if a key already exists in the dictionary. This will most likely happen when useFullName is false and two different nodes have the same name.</param>
        public static void RegisterType(Type type, string name = null, bool useFullName = false, bool failOnCollision = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = useFullName? type.FullName : type.Name;
            }

            bool added = _typeCache.TryAdd(name, type);
            if (!added) 
            {
                if(failOnCollision)
                    throw new ArgumentException("An element with the same key already exists in the abbreviation registry.");

                _typeCache.AddOrUpdate(name, type, (key, oldValue) => type);
            }

            added = _nameCache.TryAdd(type, name);

            if (!added)
            {
                if (failOnCollision)
                    throw new ArgumentException("An element with the same key already exists in the abbreviation registry.");

                _nameCache.AddOrUpdate(type, name, (key, oldVal) => name);
            }
        }

        /// <summary>
        /// Register node types from the provided assembly.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        /// <param name="useFullName">Uses the full name when registering so that it doesn't collide with an existing type name if needed.</param>
        /// <param name="failOnCollision">Fail if a key already exists in the dictionary. This will most likely happen when useFullName is false and two different nodes have the same name.</param>
        public static void RegisterFromAssembly(Assembly assembly, bool useFullName = false, bool failOnCollision = true)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces()
                .Any(y => y.IsGenericType && (y.GetGenericTypeDefinition() == typeof(INode<>) || y.GetGenericTypeDefinition() == typeof(IShouldExecuteBlock<>))));
            
            foreach (var type in types)
            {
                RegisterType(type, useFullName: useFullName, failOnCollision:failOnCollision);
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