namespace Banzai.Factories
{
    public interface IComponentSerializer
    {
        string Serialize<T>(FlowComponent<T> component);

        FlowComponent<T> Deserialize<T>(string body);
    }
}