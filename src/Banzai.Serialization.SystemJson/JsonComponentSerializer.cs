using System.Text.Json;
using System.Text.Json.Serialization;
using Banzai.Factories;

namespace Banzai.Serialization.SystemJson
{
    /// <summary>
    ///     Serializes a component as Json
    /// </summary>
    public class JsonComponentSerializer : IComponentSerializer
    {
        private readonly JsonSerializerOptions _options = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            ReferenceHandler = ReferenceHandler.Preserve,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        /// <summary>
        ///     Initializes a new instance of JsonComponentSerializer
        /// </summary>
        public JsonComponentSerializer()
        {
            _options.Converters.Add(new TypeJsonConverter());
        }

        /// <summary>
        ///     Serialize the a FlowComponent to JSON
        /// </summary>
        /// <param name="component">FlowComponent to serialize</param>
        /// <typeparam name="T">Type of FlowComponent to serialize</typeparam>
        /// <returns>JSON representation of FLowComponent</returns>
        public string Serialize<T>(FlowComponent<T> component)
        {
            return JsonSerializer.Serialize(component, _options);
        }

        /// <summary>
        ///     Deserialize the JSON to a FlowComponent
        /// </summary>
        /// <param name="body">JSON to deserialize</param>
        /// <typeparam name="T">Type of FlowComponent to create</typeparam>
        /// <returns>Deserialized FlowComponent</returns>
        public FlowComponent<T> Deserialize<T>(string body)
        {
            return JsonSerializer.Deserialize<FlowComponent<T>>(body, _options);
        }
    }
}