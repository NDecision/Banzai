using Autofac;
using Banzai.Factories;

namespace Banzai.Autofac
{
    public class AutofacFlowRegistrar : IFlowRegistrar
    {
        private readonly ContainerBuilder _containerBuilder;

        public AutofacFlowRegistrar(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void RegisterFlow<T>(FlowComponent<T> flowRoot)
        {
            _containerBuilder.RegisterInstance(flowRoot)
                .AsSelf()
                .Named<FlowComponent<T>>(flowRoot.Name);
        }
    }
}