using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XenaExchange.Client.Serialization
{
    /// <summary>
    /// Replaces the regular json names with those provided in the dictionary.
    /// </summary>
    public class CustomJsonNameContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<string, Dictionary<string, string>> _jsonNamesDictionary;

        public CustomJsonNameContractResolver(Dictionary<string, Dictionary<string, string>> jsonNamesDictionary)
        {
            _jsonNamesDictionary = jsonNamesDictionary;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.PropertyType == typeof(string))
            {
                // Don't serialize empty strings cause they are default values for protobuf messages.
                property.ShouldSerialize = instance =>
                {   
                    var v = instance.GetType().GetProperty(member.Name).GetValue(instance, null) as string;
                    return !string.IsNullOrWhiteSpace(v);
                };
            }

            if (!_jsonNamesDictionary.TryGetValue(member.DeclaringType.FullName, out var jsonNames))
                return property;

            // Replace json property name with custom one.
            if (jsonNames.TryGetValue(member.Name, out var customName))
                property.PropertyName = customName;

            return property;
        }
    }
}