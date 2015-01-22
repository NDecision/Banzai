using System;

namespace Banzai.Serialization
{
    public static class SerializerProvider
    {
        private static IComponentSerializer _serializer;

        public static IComponentSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                    throw new NullReferenceException("The Serializer has not been set...did you install a serializer package (like Banzai.JSON) and call RegisterAsDefault?");
                return _serializer;
            }
            set { _serializer = value; }
        } 
    }
}