using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Banzai.Serialization.JsonNet
{
    /// <inheritdoc />
    public class TypeJsonConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var aggregateType = (Type) value;
            string serializationString;

            if (aggregateType.IsGenericType)
            {
                var nodeType = aggregateType.GetGenericTypeDefinition();
                var argType = aggregateType.GetGenericArguments()[0];

                var argString = GetTypeName(argType);
                var nodeString = GetTypeName(nodeType);
                nodeString = nodeString.Substring(0, nodeString.IndexOf("`", StringComparison.Ordinal));

                serializationString = $"{nodeString}[{argString}]";
            }
            else
            {
                serializationString = GetTypeName((Type) value);
            }

            serializer.Serialize(writer, serializationString);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            string typeName;

            if (reader.ValueType == typeof(string))
            {
                typeName = (string) reader.Value;
            }
            else
            {
                var jsonObject = JObject.Load(reader);
                var properties = jsonObject.Properties().ToList();

                typeName = (string) properties[0].Value;
            }

            var typeNames = typeName.Split(new[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);

            var nodeTypeName = typeNames[0];
            var isGeneric = typeNames.Length > 1;
            Type nodeType;

            if (isGeneric)
            {
                nodeTypeName += "`" + (typeNames.Length - 1);
                nodeType = GetType(nodeTypeName);
                var subjectTypeName = typeNames[1];
                var subjectType = GetType(subjectTypeName);
                nodeType = nodeType.MakeGenericType(subjectType);
            }
            else
            {
                nodeType = GetType(nodeTypeName);
            }

            return nodeType;
        }

        /// <summary>
        ///     Determines if the type converter can convert this type
        /// </summary>
        /// <param name="objectType">The type of the object under consideration</param>
        /// <returns>Boolean indicating if the item can be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Type));
        }

        private static string GetTypeName(Type type)
        {
            if (!TypeAbbreviationCache.TryGetName(type, out var retrievedName))
            {
                retrievedName = type.AssemblyQualifiedName;
            }

            return retrievedName;
        }

        private static Type GetType(string typeName)
        {
            if (!TypeAbbreviationCache.TryGetType(typeName, out var retrievedType))
            {
                retrievedType = Type.GetType(typeName);
            }

            return retrievedType;
        }
    }
}