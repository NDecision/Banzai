namespace Banzai.Serialization.SystemJson
{
    /// <summary>
    ///     Registers this serializer with the Banzai framework
    /// </summary>
    public static class Registrar
    {
        /// <summary>
        ///     Register this provider as the default serializer
        /// </summary>
        public static void RegisterAsDefault()
        {
            SerializerProvider.Serializer = new JsonComponentSerializer();
        }
    }
}