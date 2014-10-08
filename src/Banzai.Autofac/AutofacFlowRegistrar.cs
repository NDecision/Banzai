using Autofac;
using Banzai.Factories;

namespace Banzai.Autofac
{
    /// <summary>
    /// Allows flows to be registered with Autofac.
    /// </summary>
    public class AutofacFlowRegistrar : IFlowRegistrar
    {
        private readonly ContainerBuilder _containerBuilder;

        /// <summary>
        /// Creates a new AutofacFlowRegistrar.
        /// </summary>
        /// <param name="containerBuilder">Autofac ContainerBuilder to use in building flows.</param>
        public AutofacFlowRegistrar(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        /// <summary>
        /// Registers the constructed flow with Autofac.
        /// </summary>
        /// <typeparam name="T">Type of the flows subject.</typeparam>
        /// <param name="flowRoot">Root of the flow to register.</param>
        public void RegisterFlow<T>(FlowComponent<T> flowRoot)
        {
            _containerBuilder.RegisterInstance(flowRoot)
                .AsSelf()
                .Named<FlowComponent<T>>(flowRoot.Name);
        }
    }
}