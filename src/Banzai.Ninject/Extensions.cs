using System.Reflection;
using Banzai.Ninject.Utility;
using Ninject;
using Ninject.Extensions.Conventions;
using Banzai.Factories;

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
                        .SelectAllClasses().Where(t => t.IsAssignableFrom(typeof(INode<>)) 
                            && !t.IsAssignableFrom(typeof(IMultiNode<>)) && !t.InheritsOrImplements(typeof(ITransitionNode<,>)))
                        .BindAllInterfaces()
                        .Configure(c => c.InSingletonScope());
                    k.FromThisAssembly()
                        .SelectAllClasses().Where(t => t.IsAssignableFrom(typeof(INode<>))
                            && !t.IsAssignableFrom(typeof(IMultiNode<>)) && !t.InheritsOrImplements(typeof(ITransitionNode<,>)))
                        .BindToSelf()
                        .Configure(c => c.InSingletonScope());
                });

                builder.Bind(k =>
                {
                    k.From(assembly)
                        .SelectAllClasses().Where(t => t.IsAssignableFrom(typeof(IMultiNode<>)) || t.InheritsOrImplements(typeof(ITransitionNode<,>)))
                        .BindAllInterfaces()
                        .Configure(c => c.InSingletonScope()
                        .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
                    k.FromThisAssembly()
                        .SelectAllClasses().Where(t => t.IsAssignableFrom(typeof(IMultiNode<>)) || t.InheritsOrImplements(typeof(ITransitionNode<,>)))
                        .BindToSelf()
                        .Configure(c => c.InSingletonScope()
                        .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
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

            builder.Bind(k =>
            {
                k.FromThisAssembly()
                    .SelectAllClasses()
                    .BindAllInterfaces()
                    .Configure(c => c.InSingletonScope());
                k.FromThisAssembly()
                    .SelectAllClasses()
                    .BindToSelf()
                    .Configure(c => c.InSingletonScope());
            });

            builder.Bind(k =>
            {
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(GroupNode<>))
                    .BindAllInterfaces()
                    .Configure(c => c.InSingletonScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(GroupNode<>))
                    .BindToSelf()
                    .Configure(c => c.InSingletonScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
            });

            builder.Bind(k =>
            {
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(PipelineNode<>))
                    .BindAllInterfaces()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(PipelineNode<>))
                    .BindToSelf()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
            });

            builder.Bind(k =>
            {
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(FirstMatchNode<>))
                    .BindAllInterfaces()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(FirstMatchNode<>))
                    .BindToSelf()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
            });

            builder.Bind(k =>
            {
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(TransitionFuncNode<,>))
                    .BindAllInterfaces()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
                k.FromAssemblyContaining<NodeResult>()
                    .Select(type => type == typeof(TransitionFuncNode<,>))
                    .BindToSelf()
                    .Configure(c => c.InTransientScope()
                    .WithPropertyValue("NodeFactory", ctxt => ctxt.Kernel.Get(typeof(INodeFactory<>))));
            });


            return builder;
        }


    }
}