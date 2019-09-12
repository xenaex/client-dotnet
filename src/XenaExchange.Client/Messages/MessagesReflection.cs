using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using XenaExchange.Client.Messages.Constants;

namespace XenaExchange.Client.Messages
{
    /// <summary>
    /// Class for retrieving messages serialization info using reflection.
    /// </summary>
    public static class MessagesReflection
    {
        private class DescriptionWrapper
        {
            public readonly string TypeName;
            public readonly MessageDescriptor Descriptor;

            public DescriptionWrapper(string typeName, MessageDescriptor descriptor)
            {
                TypeName = typeName;
                Descriptor = descriptor;
            }
        }

        /// <summary>
        /// Gets all Xena protobuf messages using reflection and build fix tags mapping for usual fields.
        /// *Fix tag for the field is a protobuf field number.
        /// </summary>
        /// <returns>Dictionary{TypeName, Dictionary{FieldName, FixTag}}</returns>
        public static Dictionary<string, Dictionary<string, string>> FixJsonNames()
        {
            // Type -> Field name -> Field number
            var fixDictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (var descriptorWrapper in EnumerateDescriptors())
            {
                var fixTags = new Dictionary<string, string>();
                foreach (var field in descriptorWrapper.Descriptor.Fields.InDeclarationOrder())
                    fixTags.Add(field.Name, field.FieldNumber.ToString());

                fixDictionary.Add(descriptorWrapper.TypeName, fixTags);
            }

            return fixDictionary;
        }

        /// <summary>
        /// Gets all Xena protobuf messages using reflection and build json names mapping for usual fields.
        /// </summary>
        /// <returns>Dictionary{TypeName, Dictionary{FieldName, JsonName}}</returns>
        public static Dictionary<string, Dictionary<string, string>> JsonNames()
        {
            // Type -> Field name -> Field json name
            var jsonNamesDictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (var descriptorWrapper in EnumerateDescriptors())
            {
                var jsonNames = new Dictionary<string, string>();
                foreach (var field in descriptorWrapper.Descriptor.Fields.InDeclarationOrder())
                    jsonNames.Add(field.Name, field.JsonName);

                jsonNamesDictionary.Add(descriptorWrapper.TypeName, jsonNames);
            }

            return jsonNamesDictionary;
        }

        private static IEnumerable<DescriptionWrapper> EnumerateDescriptors()
        {
            return Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => typeof(IMessage).IsAssignableFrom(t))
                .Select(t => t.GetProperty(nameof(Logon.Descriptor), BindingFlags.Public | BindingFlags.Static))
                .Select(f => new DescriptionWrapper(f.DeclaringType.FullName, (MessageDescriptor) f.GetValue(null)));
        }
    }
}