using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banzai.Serialization.SystemJson
{
    // Sample workaround for System.Type with S.T.Json using converters
    /// <inheritdoc />
    public class TypeJsonConverter : JsonConverter<Type>
    {
        /// <inheritdoc />
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var typeName = reader.GetString();

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

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            string serializationString;

            if (value.IsGenericType)
            {
                var nodeType = value.GetGenericTypeDefinition();
                var argType = value.GetGenericArguments()[0];

                var argString = GetTypeName(argType);
                var nodeString = GetTypeName(nodeType);
                nodeString = nodeString.Substring(0, nodeString.IndexOf("`", StringComparison.Ordinal));

                serializationString = $"{nodeString}[{argString}]";
            }
            else
            {
                serializationString = GetTypeName(value);
            }

            writer.WriteStringValue(serializationString);
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