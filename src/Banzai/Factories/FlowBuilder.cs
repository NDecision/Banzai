namespace Banzai.Factories
{
    /// <summary>
    /// Root class for building flows.
    /// </summary>
    /// <typeparam name="T">Subject type for the flow to build.</typeparam>
    public sealed class FlowBuilder<T>
    {
        private FlowComponent<T> _rootComponent;
        private readonly IFlowRegistrar _flowRegistrar;

        /// <summary>
        /// Constructs a new FlowBuilder.
        /// </summary>
        /// <param name="flowRegistrar">A registrar typically provided by a DI container that this flow can be registered with.</param>
        public FlowBuilder(IFlowRegistrar flowRegistrar)
        {
            _flowRegistrar = flowRegistrar;
        }

        /// <summary>
        /// Creates a flow of the specified name and current subject type (T).
        /// </summary>
        /// <param name="name">Name of the flow.</param>
        /// <returns></returns>
        public IFlowBuilder<T> CreateFlow(string name)
        {
            _rootComponent = new FlowComponent<T> { Type = typeof(T), Name = name, IsFlow = true };

            return new FlowComponentBuilder<T>(_rootComponent);
        }

        /// <summary>
        /// Registers the flow with the IFlowRegistrar.
        /// </summary>
        public void Register()
        {
            _flowRegistrar.RegisterFlow(_rootComponent);
        }

    }
}