namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Projapocsur.Common;

    /// <summary>
    /// Deserializes XML written by <see cref="XmlSerializer"/>.
    /// </summary>
    public class XmlDeserializer
    {
        private static IReadOnlyDictionary<Type, Func<string, dynamic>> SimpleDeserializationStrategies =
            new Dictionary<Type, Func<string, dynamic>>()
        {
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
            { typeof(object), (value) => new object() },
            { typeof(decimal), (value) => decimal.Parse(value) },
            { typeof(string), (value) => value },
        };

        private XmlDeserializer() { }

        public static bool CanDeserialize(Type type)
        {
            return SimpleDeserializationStrategies.ContainsKey(type)
                || type.GetCustomAttribute<XmlSerializableAttribute>() != null
                || (ReflectionUtility.IsList(type, out Type innerType) && CanDeserialize(innerType));
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
            var memberByName = new Dictionary<string, XmlSerializableMember>();
            IEnumerable<XmlSerializableMember> serializableMembers = XmlSerializableMemberFactory.GetSerializableMembers(data);

            serializableMembers.ForEach((member) => memberByName[member.XmlMemberAttribute.PreferredName ?? member.Name] = member);
            this.Deserialize(reader, data.GetType().Name, memberByName);
        }

        private void Deserialize(XmlReader reader, string parentTypeName, Dictionary<string, XmlSerializableMember> memberByName)
        {
            string currentElementName = string.Empty;
            string parentElementName = null;

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElementName = reader.Name;
                        parentElementName ??= reader.Name;
                        bool isEmptyRootElement = currentElementName == parentElementName && reader.IsEmptyElement;    // empty flag captured here before pivot gets moved to attributes, if there are any
                        this.DeserializeNestedObject(parentTypeName: parentTypeName, currentElementName: currentElementName, memberByNameLookup: memberByName, reader: reader);
                        this.DeserializeAttributes(parentTypeName: parentTypeName, currentElementName: currentElementName, parentElementName: parentElementName, memberByName, reader);
                        if (isEmptyRootElement) { return; }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == parentElementName) { return; } // incase a nested object list pushes to the end element 

                        break;
                    case XmlNodeType.Text:
                        this.DeserializeNode(parentTypeName: parentTypeName, name: currentElementName, value: reader.Value, memberByName);
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == parentElementName || parentElementName == null) { return; }

                        break;
                }
            } while (reader.Read());
        }

        private void DeserializeNestedObject(
            string parentTypeName,
            string currentElementName,
            Dictionary<string, XmlSerializableMember> memberByNameLookup,
            XmlReader reader)
        {
            if (memberByNameLookup.TryGetValue(reader.Name, out XmlSerializableMember member)
                && (!SimpleDeserializationStrategies.ContainsKey(member.ValueType)))
            {
                if (member.HasXmlSerializableAttribute)
                {
                    this.Deserialize(reader, member.Value);
                }
                else if (ReflectionUtility.IsList(member.ValueType, out Type innerType) && CanDeserialize(innerType))
                {
                    this.DeserializeList(reader, member.Value, innerType, currentElementName);
                }
                else
                {
                    throw new XmlUnsupportedTypeException(member.ValueType, member.Name, parentTypeName);
                }
            }
        }

        private void DeserializeList(XmlReader reader, object target, Type innerType, string currentElementName)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string targetTypeName = target.GetType().Name;
            dynamic list = target;
            list.Clear();
            var memberByName = new Dictionary<string, XmlSerializableMember>();

            while (!(reader.Name == currentElementName && reader.NodeType == XmlNodeType.EndElement) 
                && reader.NodeType != XmlNodeType.None)
            {
                // move to first element in list
                reader.Read(); 
                while (reader.NodeType != XmlNodeType.Element) 
                { 
                    reader.Read(); 
                    if (reader.Name == currentElementName)  //exit on end tag of list
                    {
                        break;
                    }
                }

                // deserialize element
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var member = new XmlSerializableMember(reader.Name, innerType, null);
                    dynamic value = member.Value;
                    memberByName[reader.Name] = member;
                    this.Deserialize(reader, targetTypeName, memberByName);
                    value = member.Value;
                    list.Add(value);
                }
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
                    this.DeserializeNode(parentTypeName: parentTypeName, name: reader.Name, value: reader.Value, memberByNameLookup);
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
                if (SimpleDeserializationStrategies.TryGetValue(member.ValueType, out Func<string, dynamic> deserializeString))
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
