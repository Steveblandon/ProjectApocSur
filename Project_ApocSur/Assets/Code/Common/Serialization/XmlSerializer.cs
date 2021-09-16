namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Projapocsur.Common.Extensions;

    /// <summary>
    /// Custom XMLSerializer that unlike the .NET serializers allows for the serialization of any field/property marked 
    /// with <see cref="XmlMemberAttribute"/>, no matter what it's access level is.
    /// Other than primitive types, the only complex types supported at the moment are custom classes marked with the
    /// <see cref="XmlSerializableAttribute"/>.
    /// </summary>
    public class XmlSerializer
    {
        /// <summary>
        /// used for tracking types while parsing nested nodes to detect serializable circular dependencies and avoid causing stack overflows.
        /// </summary>
        private ISet<Type> nestedTypes = new HashSet<Type>();

        private XmlSerializer() 
        {
            nestedTypes = new HashSet<Type>();
        }

        public static void Serialize(Stream stream, object data)
        {
            if (data.GetType().GetCustomAttribute<XmlSerializableAttribute>() == null)
            {
                throw new XmlUnsupportedTypeException(data.GetType(), nameof(data), nameof(Serialize));
            }

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer();
                serializer.nestedTypes.Add(data.GetType());
                serializer.Serialize(writer, data);
            }
        }

        private void Serialize(XmlWriter writer, object obj, string rootName = null)
        {
            Type objType = obj.GetType();
            string currentNodeName = rootName ?? objType.Name;

            if (obj.GetType().IsPrimitive())
            {
                this.SerializeAsElement(writer, currentNodeName, obj);
                return;
            }

            LinkedList<XmlSerializableMember> serializableMembers = XmlSerializableMemberFactory.GetSerializableMembers(obj);

            writer.WriteStartElement(currentNodeName);
            SerializeMembers(writer, objType, serializableMembers);
            writer.WriteEndElement();
        }

        private void SerializeMembers(XmlWriter writer, Type objType, LinkedList<XmlSerializableMember> serializableMembers)
        {
            foreach (var member in serializableMembers)
            {
                string finalName = member.XmlMemberAttribute.PreferredName ?? member.Name;

                if (member.HasXmlSerializableAttribute)
                {
                    SerializeNestedObject(writer, member, name: finalName, parentTypeName: objType.Name);
                }
                else if (member.ValueType.IsPrimitive())
                {
                    if (member.XmlMemberAttribute.IsAttribute)
                    {
                        this.SerializeAsAttribute(writer, finalName, member.Value);
                    }
                    else
                    {
                        this.SerializeAsElement(writer, finalName, member.Value);
                    }
                }
                else
                {
                    throw new XmlUnsupportedTypeException(member.ValueType, member.Name, source: objType.Name);
                }
            }
        }

        private void SerializeNestedObject(XmlWriter writer, XmlSerializableMember member, string name, string parentTypeName)
        {
            if (nestedTypes.Contains(member.ValueType))
            {
                throw new XmlInvalidException("circular dependency detected", member.ValueType, member.Name, source: parentTypeName);
            }

            nestedTypes.Add(member.ValueType);
            Serialize(writer, member.Value, name);
        }

        private void SerializeAsElement(XmlWriter writer, string name, object value)
        {
            writer.WriteStartElement(name);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        private void SerializeAsAttribute(XmlWriter writer, string name, object value)
        {
            writer.WriteStartAttribute(name);
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }
    }
}
