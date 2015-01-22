using Banzai.Serialization;

namespace Banzai.Json
{
    public static class Registrar
    {
        public static void RegisterAsDefault()
        {
            SerializerProvider.Serializer = new JsonComponentSerializer();
        }
    }
}