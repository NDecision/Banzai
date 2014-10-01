using System.Linq;
using System.Reflection;
using Autofac;

namespace Banzai.Autofac
{
    public static class Extensions
    {
        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <param name="assemblies">Assemblies to scan.</param>
        /// <param name="includeCore">Option to include core Banzai assembly registration.</param>
        /// <returns>Builder including added nodes.</returns>
        public static ContainerBuilder RegisterBanzaiNodes(this ContainerBuilder builder, Assembly[] assemblies, bool includeCore = false)
        {
            if (assemblies == null)
                return builder;

            foreach (var assembly in assemblies)
            {
                builder.RegisterBanzaiNodes(assembly);
            }

            if (includeCore)
            {
                builder.RegisterBanzaiNodes();
            }

            return builder;
        }


        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <param name="assembly">Assembly to scan.</param>
        /// <param name="includeCore">Option to include core Banzai assembly registration.</param>
        /// <returns>Builder including added nodes.</returns>
        public static ContainerBuilder RegisterBanzaiNodes(this ContainerBuilder builder, Assembly assembly, bool includeCore = false)
        {
            if (assembly == null)
                return builder;

            var types = assembly.GetTypes().Where(t => t.IsClass && t.IsClosedTypeOf(typeof (INode<>)));

            foreach (var type in types)
            {
                if (type.IsGenericTypeDefinition)
                {
                    builder.RegisterGeneric(type)
                        .AsSelf()
                        .AsImplementedInterfaces()
                        .InstancePerDependency();
                }
                else
                {
                    builder.RegisterType(type)
                        .AsSelf()
                        .AsImplementedInterfaces()
                        .InstancePerDependency();
                }
            }

            if (includeCore)
            {
                builder.RegisterBanzaiNodes();
            }

            return builder;
        }


        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <typeparam name="T">Type form which to derive the assembly to scan.</typeparam>
        /// <param name="includeCore">Option to include core Banzai assembly registration.</param>
        /// <returns>Builder including added nodes.</returns>
        public static ContainerBuilder RegisterBanzaiNodes<T>(this ContainerBuilder builder, bool includeCore = false)
        {
            return builder.RegisterBanzaiNodes(typeof(T).Assembly, includeCore);
        }

        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <returns>Builder including added nodes.</returns>
        public static ContainerBuilder RegisterBanzaiNodes(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GroupNode<>))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(PipelineNode<>))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(FirstMatchNode<>))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(AutofacNodeFactory<>))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            return builder;
        }

    }
}