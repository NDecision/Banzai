using System.Linq;
using System.Reflection;
using Banzai.Ninject.Utility;
using Ninject;
using Ninject.Extensions.Conventions;
using Banzai.Factories;
using Ninject.Selection.Heuristics;

namespace Banzai.Ninject
{
    /// <summary>
    /// Extensions for assistence in registering nodes via Ninject.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <param name="assemblies">Assemblies to scan.</param>
        /// <param name="includeCore">Option to include core Banzai assembly registration.</param>
        /// <returns>Builder including added nodes.</returns>
        public static IKernel RegisterBanzaiNodes(this IKernel builder, Assembly[] assemblies, bool includeCore = false)
        {
            if (includeCore)
            {
                builder.RegisterBanzaiNodes();
            }

            if (assemblies != null)
            {
                foreach (var assembly in assemblies)
                {
                    builder.RegisterBanzaiNodes(assembly);
                }
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
        public static IKernel RegisterBanzaiNodes(this IKernel builder, Assembly assembly, bool includeCore = false)
        {
            if (includeCore)
            {
                builder.RegisterBanzaiNodes();
            }

            if (assembly != null)
            {
                builder.Bind(k =>
                {
                    k.From(assembly)
                        .SelectAllClasses().Where(t => t.InheritsOrImplements(typeof(INode<>)))
                        .BindAllInterfaces()
                        .Configure(c => c.InTransientScope());
                    k.FromThisAssembly()
                        .SelectAllClasses().Where(t => t.InheritsOrImplements(typeof(INode<>)))
                        .BindToSelf()
                        .Configure(c => c.InTransientScope());
                    k.From(assembly)
                        .SelectAllClasses().Where(t => t.InheritsOrImplements(typeof(IMetaDataBuilder)))
                        .BindAllInterfaces()
                        .Configure(c => c.InTransientScope());
                });
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
        public static IKernel RegisterBanzaiNodes<T>(this IKernel builder, bool includeCore = false)
        {
            return builder.RegisterBanzaiNodes(typeof(T).Assembly, includeCore);
        }

        /// <summary>
        /// Scans the provided assemblly for INodes.  Registers the node as self and as I{NodeClassName}
        /// </summary>
        /// <param name="builder">Builder to register nodes with.</param>
        /// <returns>Builder including added nodes.</returns>
        public static IKernel RegisterBanzaiNodes(this IKernel builder)
        {
            builder.Bind(typeof (INodeFactory<>)).To(typeof(NinjectNodeFactory<>)).InSingletonScope();
            builder.Bind<INodeFactory>().To<NinjectNodeFactory>().InSingletonScope();
            builder.Bind<IFlowRegistrar>().To<NinjectFlowRegistrar>().InSingletonScope();

            builder.Components.Add<IInjectionHeuristic, NodeFactoryInjectionHeuristic>();

            builder.Bind(k => k.FromAssemblyContaining<NodeResult>()
                .Select(type => type == typeof(GroupNode<>))
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope()));

            builder.Bind(k => k.FromAssemblyContaining<NodeResult>()
                .Select(type => type == typeof(PipelineNode<>))
                .BindAllInterfaces()
                .Configure(c => c.InTransientScope()));

            builder.Bind(k => k.FromAssemblyContaining<NodeResult>()
                .Select(type => type == typeof(FirstMatchNode<>))
                .BindAllInterfaces()
                .Configure(c => c.InTransientScope()));

            builder.Bind(k => k.FromAssemblyContaining<NodeResult>()
                .Select(type => type == typeof(TransitionFuncNode<,>))
                .BindAllInterfaces()
                .Configure(c => c.InTransientScope()));


            return builder;
        }


    }
}