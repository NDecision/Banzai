using System;
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
                if (property.PropertyName == "Id")
                {
                    //Don't serialize the ID if it's null or the default
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetTypeInfo().GetProperty(property.PropertyName);
                            var id = (string)prop.GetValue(instance);

                            if (string.IsNullOrEmpty(id))
                                return false;

                            var type = (Type)property.DeclaringType.GetTypeInfo().GetProperty("Type").GetValue(instance);
                            var name = (string)property.DeclaringType.GetTypeInfo().GetProperty("Name").GetValue(instance);

                            if (type == null)
                                return false;

                            var compareString = type.FullName;
                            if (!string.IsNullOrEmpty(name))
                                compareString += ":" + name;

                            return compareString != id;
                        };
                }
                else if (property.PropertyName == "Children")
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetTypeInfo().GetProperty(property.PropertyName);
                            object data = prop.GetValue(instance);

                            return data != null && ((IList) data).Count > 0;
                        };
                }
                else if (property.PropertyName == "MetaData")
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetTypeInfo().GetProperty(property.PropertyName);
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