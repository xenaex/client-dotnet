using System.Collections;
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
                    var p = instance.GetType().GetProperty(member.Name);
                    if (p == null)
                        return false;

                    return !string.IsNullOrWhiteSpace(p.GetValue(instance, null) as string);
                };
            }
            else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    var p = instance.GetType().GetProperty(member.Name);
                    if (p == null)
                        return false;

                    var e = p.GetValue(instance, null) as IEnumerable;
                    return e != null && e.GetEnumerator().MoveNext();
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