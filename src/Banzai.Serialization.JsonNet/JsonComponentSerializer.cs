using System.IO;
using Banzai.Factories;
using Newtonsoft.Json;

namespace Banzai.Serialization.JsonNet
{
    /// <inheritdoc />
    public class JsonComponentSerializer : IComponentSerializer
    {
        private static readonly JsonSerializer _serializer;

        static JsonComponentSerializer()
        {
            TypeAbbreviationCache.RegisterCoreTypes();

            _serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new JsonContractResolver()
            };
        }


        /// <inheritdoc />
        public string Serialize<T>(FlowComponent<T> component)
        {
            using (var sw = new StringWriter())
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                _serializer.Serialize(writer, component);

                return sw.ToString();
            }
        }

        /// <inheritdoc />
        public FlowComponent<T> Deserialize<T>(string body)
        {
            using (var reader = new JsonTextReader(new StringReader(body)))
            {
                return _serializer.Deserialize<FlowComponent<T>>(reader);
            }
        }
    }
}