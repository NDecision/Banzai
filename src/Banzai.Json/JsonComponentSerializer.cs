using Banzai.Factories;
using Newtonsoft.Json;

namespace Banzai.Json
{
    public class JsonComponentSerializer : IComponentSerializer
    {
        public string Serialize<T>(FlowComponent<T> component)
        {
            return JsonConvert.SerializeObject(component);
        }

        public FlowComponent<T> Deserialize<T>(string body)
        {
            return JsonConvert.DeserializeObject<FlowComponent<T>>(body);
        }
    }
}
