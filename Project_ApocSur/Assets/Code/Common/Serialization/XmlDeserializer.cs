namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Projapocsur.Common.Extensions;

    /// <summary>
    /// Deserializes XML written by <see cref="XmlSerializer"/>.
    /// </summary>
    public class XmlDeserializer
    {
        private static IReadOnlyDictionary<Type, Func<string, dynamic>> DeserializationStrategies =
            new Dictionary<Type, Func<string, dynamic>>()
        {
            // primitives
            { typeof(bool), (value) => bool.Parse(value) },
            { typeof(byte), (value) => byte.Parse(value) },
            { typeof(sbyte), (value) => sbyte.Parse(value) },
            { typeof(char), (value) => char.Parse(value) },
            { typeof(short), (value) => short.Parse(value) },
            { typeof(ushort), (value) => ushort.Parse(value) },
            { typeof(int), (value) => int.Parse(value) },
            { typeof(uint), (value) => uint.Parse(value) },
            { typeof(long), (value) => long.Parse(value) },
            { typeof(ulong), (value) => ulong.Parse(value) },
            { typeof(float), (value) => float.Parse(value) },
            { typeof(double), (value) => double.Parse(value) },

            //NOTE: current commented out entries need to be tested, should be tested once we start to add support for built-in complex types

            // complex objects
            { typeof(object), (value) => new object() },
            /*{ typeof(decimal), (value) => decimal.Parse(value) },*/
            { typeof(string), (value) => value },
            /*{ typeof(DateTimeOffset), (value) => DateTimeOffset.Parse(value) },
            { typeof(DateTime), (value) => DateTime.Parse(value) },*/
        };

        private XmlDeserializer() { }

        public static bool CanDeserialize(Type type)
        {
            return DeserializationStrategies.ContainsKey(type) || type.GetCustomAttribute<XmlSerializableAttribute>() != null;
        }

        public static void Deserialize<T>(Stream stream, out T data) where T : new()
        {
            data = new T();

            if (data.GetType().GetCustomAttribute<XmlSerializableAttribute>() == null)
            {
                throw new XmlUnsupportedTypeException(data.GetType(), nameof(data), nameof(Deserialize));
            }

            using (XmlReader reader = XmlReader.Create(stream))
            {
                var deserializer = new XmlDeserializer();
                deserializer.Deserialize(reader, data);
            }
        }

        private void Deserialize(XmlReader reader, object data)
        {
            string dataTypeName = data.GetType().Name;
            string currentElementName = string.Empty;
            string parentElementName = null;
            var memberByName = new Dictionary<string, XmlSerializableMember>();
            IEnumerable<XmlSerializableMember> serializableMembers = XmlSerializableMemberFactory.GetSerializableMembers(data);

            serializableMembers.ForEach((member) => memberByName[member.XmlMemberAttribute.PreferredName ?? member.Name] = member);

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElementName = reader.Name;
                        parentElementName ??= reader.Name;
                        bool isEmptyRootElement = currentElementName == parentElementName && reader.IsEmptyElement;    // empty flag captured here before pivot gets moved to attributes, if there are any
                        DeserializeNestedObject(parentTypeName: dataTypeName, currentElementName: currentElementName, parentElementName: parentElementName, memberByName, reader);
                        DeserializeAttributes(parentTypeName: dataTypeName, currentElementName: currentElementName, parentElementName: parentElementName, memberByName, reader);
                        if (isEmptyRootElement) { return; }
                        break;
                    case XmlNodeType.Text:
                        DeserializeNode(parentTypeName: dataTypeName, name: currentElementName, value: reader.Value, memberByName);
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == parentElementName) { return; }
                        break;
                }
            } while (reader.Read());
        }

        private void DeserializeNestedObject(
            string parentTypeName,
            string currentElementName, 
            string parentElementName, 
            Dictionary<string, XmlSerializableMember> memberByNameLookup, 
            XmlReader reader)
        {
            if (currentElementName != parentElementName
                && memberByNameLookup.TryGetValue(reader.Name, out XmlSerializableMember member)
                && (!DeserializationStrategies.ContainsKey(member.ValueType)))
            {
                if (!member.HasXmlSerializableAttribute)
                {
                    throw new XmlUnsupportedTypeException(member.ValueType, member.Name, parentTypeName);
                }

                Deserialize(reader, member.Value);
            }
        }

        private void DeserializeAttributes(
            string parentTypeName,
            string currentElementName, 
            string parentElementName, 
            Dictionary<string, XmlSerializableMember> memberByNameLookup, 
            XmlReader reader)
        {
            if (reader.HasAttributes && currentElementName == parentElementName)
            {
                while (reader.MoveToNextAttribute())
                {
                    DeserializeNode(parentTypeName: parentTypeName, name: reader.Name, value: reader.Value, memberByNameLookup);
                }
            }
        }

        private void DeserializeNode(
            string parentTypeName,
            string name,
            string value,
            Dictionary<string, XmlSerializableMember> memberByNameLookup)
        {
            if (memberByNameLookup.TryGetValue(name, out XmlSerializableMember member))
            {
                if (DeserializationStrategies.TryGetValue(member.ValueType, out Func<string, dynamic> deserializeString))
                {
                    member.Value = deserializeString(value);
                }
                else
                {
                    throw new XmlUnsupportedTypeException(member.ValueType, member.Name, parentTypeName);
                }
            }
            else
            {
                throw new XmlInvalidException("no matching field/property found", null, name, source: parentTypeName);
            }
        }
    }
}
