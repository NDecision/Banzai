using Banzai.Factories;
using Ninject;

namespace Banzai.Ioc.Ninject
{
    /// <summary>
    ///     Allows flows to be registered with Ninject.
    /// </summary>
    public class NinjectFlowRegistrar : IFlowRegistrar
    {
        private readonly IKernel _kernel;

        /// <summary>
        ///     Creates a new NinjectFlowRegistrar.
        /// </summary>
        /// <param name="kernel">Ninject Kernel to use in building flows.</param>
        public NinjectFlowRegistrar(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        ///     Registers the constructed flow with Ninject.
        /// </summary>
        /// <typeparam name="T">Type of the flows subject.</typeparam>
        /// <param name="flowRoot">Root of the flow to register.</param>
        public void RegisterFlow<T>(FlowComponent<T> flowRoot)
        {
            _kernel.Bind<FlowComponent<T>>().ToConstant(flowRoot)
                .Named(flowRoot.Name);
        }
    }
}