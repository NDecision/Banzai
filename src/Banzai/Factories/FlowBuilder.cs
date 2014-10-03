namespace Banzai.Factories
{
    public class FlowBuilder<T>
    {
        private FlowComponent<T> _rootComponent;
        private readonly IFlowRegistrar _flowRegistrar;

        public FlowBuilder(IFlowRegistrar flowRegistrar)
        {
            _flowRegistrar = flowRegistrar;
        }

        public IFlowBuilder<T> CreateFlow(string name)
        {
            _rootComponent = new FlowComponent<T> { Type = typeof(T), Name = name, IsFlow = true };

            return new FlowComponentBuilder<T>(_rootComponent);
        }

        public void Register()
        {
            _flowRegistrar.RegisterFlow(_rootComponent);
        }

    }
}