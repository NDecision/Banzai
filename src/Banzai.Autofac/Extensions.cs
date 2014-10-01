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
        /// <param name="assembly">Assembly to scan.</param>
        /// <returns>Builder including added nodes.</returns>
        public static ContainerBuilder ScanForNodes(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.IsClass && t.IsClosedTypeOf(typeof (INode<>)))
                .AsSelf()
                .As(t => t.GetInterfaces().Where(x => x.Name == "I" + t.Name))
                .InstancePerDependency();
            return builder;
        }
    }
}