using Banzai.Factories;

namespace Banzai.Json
{
    public static class Registrar
    {
        public static void RegisterAsDefault()
        {
            NodeFactoryBase.Serializer = new JsonComponentSerializer();
        }
    }
}