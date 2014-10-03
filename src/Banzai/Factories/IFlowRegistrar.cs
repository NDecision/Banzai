using System.Security.Cryptography.X509Certificates;

namespace Banzai.Factories
{
    public interface IFlowRegistrar
    {
        void RegisterFlow<T>(FlowComponent<T> flowRoot);
    }
}