using Banzai.Factories;

namespace Banzai.Serialization
{
    /// <summary>
    ///     Serializes and deserializes FlowComponents
    /// </summary>
    public interface IComponentSerializer
    {
        
        /// <summary>
        ///     Serialize the a FlowComponent
        /// </summary>
        /// <param name="component">FlowComponent to serialize</param>
        /// <typeparam name="T">Type of FlowComponent to serialize</typeparam>
        /// <returns>String representation of FLowComponent</returns>
        string Serialize<T>(FlowComponent<T> component);

        /// <summary>
        ///     Deserialize the string to a FlowComponent
        /// </summary>
        /// <param name="body">String to deserialize</param>
        /// <typeparam name="T">Type of FlowComponent to create</typeparam>
        /// <returns>Deserialized FlowComponent</returns>
        FlowComponent<T> Deserialize<T>(string body);
    }
}