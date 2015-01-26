using System.Collections;
using System.Reflection;
using Banzai.Factories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Banzai.Json
{
    internal class JsonContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType.GetGenericTypeDefinition() == typeof (FlowComponent<>))
            {
                if (property.PropertyName == "Children")
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetProperty(property.PropertyName);
                            object data = prop.GetValue(instance);

                            return data != null && ((IList) data).Count > 0;
                        };
                }
                else if (property.PropertyName == "MetaData")
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetProperty(property.PropertyName);
                            object data = prop.GetValue(instance);

                            return data != null && ((IDictionary)data).Count > 0;
                        };
                }
                else if (property.PropertyName.EndsWith("Type"))
                {
                    property.Converter = new TypeJsonConverter();
                    property.MemberConverter = new TypeJsonConverter();
                }
            }

            return property;
        }
    }
}