﻿using System;
using System.Collections;
using System.Reflection;
using Banzai.Factories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Banzai.Serialization.JsonNet
{
    internal class JsonContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType.GetGenericTypeDefinition() == typeof(FlowComponent<>))
            {
                if (property.PropertyName == "Id")
                {
                    //Don't serialize the ID if it's null or the default
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetProperty(property.PropertyName);
                            var id = (string) prop.GetValue(instance);

                            if (string.IsNullOrEmpty(id))
                                return false;

                            var type = (Type) property.DeclaringType.GetProperty("Type").GetValue(instance);
                            var name = (string) property.DeclaringType.GetProperty("Name").GetValue(instance);

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
                            var prop = property.DeclaringType.GetProperty(property.PropertyName);
                            var data = prop.GetValue(instance);

                            return data != null && ((IList) data).Count > 0;
                        };
                }
                else if (property.PropertyName == "MetaData")
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            var prop = property.DeclaringType.GetProperty(property.PropertyName);
                            var data = prop.GetValue(instance);

                            return data != null && ((IDictionary) data).Count > 0;
                        };
                }
                else if (property.PropertyName.EndsWith("Type"))
                {
                    property.Converter = new TypeJsonConverter();
                }
            }

            return property;
        }
    }
}