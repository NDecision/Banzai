namespace Banzai.Factories
{
    /// <summary>
    /// Registers a flow with the flow register (typically a DI container).
    /// </summary>
    public interface IFlowRegistrar
    {
        /// <summary>
        /// Registers the flow.
        /// </summary>
        /// <typeparam name="T">Type of the flow subject.</typeparam>
        /// <param name="flowRoot">Root of the flow to register.</param>
        void RegisterFlow<T>(FlowComponent<T> flowRoot);
    }
}