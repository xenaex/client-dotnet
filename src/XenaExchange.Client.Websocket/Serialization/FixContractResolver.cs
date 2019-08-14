using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XenaExchange.Client.Websocket.Serialization
{
    public class FixContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<string, Dictionary<string, string>> _fixDictionary;

        public FixContractResolver(Dictionary<string, Dictionary<string, string>> fixDictionary)
        {
            _fixDictionary = fixDictionary;
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

            if (!_fixDictionary.TryGetValue(member.DeclaringType.FullName, out var fixTags))
                return property;

            // Replace json property name with fix tag.
            if (fixTags.TryGetValue(member.Name, out var fixTag))
                property.PropertyName = fixTag;

            return property;
        }
    }
}