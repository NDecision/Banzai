using Banzai.Serialization;
using Banzai.Utility;

namespace Banzai.Factories
{
    /// <summary>
    /// Root class for building flows.
    /// </summary>
    /// <typeparam name="T">Subject type for the flow to build.</typeparam>
    public sealed class FlowBuilder<T>
    {
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
        /// <param name="id">Id of the node, if not supplied, defaults to the name.</param>
        /// <returns></returns>
        public IFlowBuilder<T> CreateFlow(string name, string id = null)
        {
            RootComponent = new FlowComponent<T> { Type = typeof(T), Name = name, Id = (string.IsNullOrEmpty(id) ? name : id), IsFlow = true };

            return new FlowComponentBuilder<T>(RootComponent);
        }

        /// <summary>
        /// Registers the flow with the IFlowRegistrar.
        /// </summary>
        public void Register()
        {
            _flowRegistrar.RegisterFlow(RootComponent);
        }

        /// <summary>
        /// Gets/Sets the root component of this flow.
        /// </summary>
        public FlowComponent<T> RootComponent { get; set; }

        /// <summary>
        /// Serializes the root component using the currently registered serializer.
        /// </summary>
        public string SerializeRootComponent()
        {
            if (RootComponent == null)
                return null;

            return SerializerProvider.Serializer.Serialize(RootComponent);
        }

        /// <summary>
        /// Deserializes the root component using the currently registered serializer.
        /// </summary>
        public FlowComponent<T> DeserializeRootComponent(string serializedComponent)
        {
            Guard.AgainstNullOrEmptyArgument("serializedComponent", serializedComponent);

            return SerializerProvider.Serializer.Deserialize<T>(serializedComponent);
        }

        /// <summary>
        /// Deserializes and sets the root component using the currently registered serializer.
        /// </summary>
        public FlowComponent<T> DeserializeAndSetRootComponent(string serializedComponent)
        {
            RootComponent = DeserializeRootComponent(serializedComponent);

            return RootComponent;
        }

    }
}