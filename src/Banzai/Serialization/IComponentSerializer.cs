using Banzai.Factories;

namespace Banzai.Serialization
{
    public interface IComponentSerializer
    {
        string Serialize<T>(FlowComponent<T> component);

        FlowComponent<T> Deserialize<T>(string body);
    }
}