namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Projapocsur.Common.Extensions;
    using Projapocsur.Common.Utilities;

    /// <summary>
    /// <para>Custom XMLSerializer that unlike the .NET serializers allows for the serialization of any field/property marked 
    /// with <see cref="XmlMemberAttribute"/>, no matter what it's access level is.
    /// </para>
    /// 
    /// NOTE: for serialization/deserialization simplicity, all fields/properties meant to be serialized, must also have a
    /// default non-null value.
    /// </summary>
    /// <remarks>
    /// Supported types: primitives, lists, and custom classes marked with the <see cref="XmlSerializableAttribute"/>
    /// </remarks>
    /// <seealso cref="XmlDeserializer"/>
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
            string objTypeName = obj.GetType().Name;
            string currentNodeName = rootName ?? objTypeName;

            if (obj.GetType().IsPrimitive())
            {
                this.SerializeAsElement(writer, currentNodeName, obj);
                return;
            }

            LinkedList<XmlSerializableMember> serializableMembers = XmlSerializableMemberFactory.GetSerializableMembers(obj);

            writer.WriteStartElement(currentNodeName);
            SerializeMembers(writer, objTypeName, serializableMembers);
            writer.WriteEndElement();
        }

        private void SerializeMembers(XmlWriter writer, string parentTypeName, LinkedList<XmlSerializableMember> serializableMembers)
        {
            foreach (var member in serializableMembers)
            {
                string finalName = member.XmlMemberAttribute.PreferredName ?? member.Name;

                if (member.XmlMemberAttribute.IsAttribute)
                {
                    this.SerializeAsAttribute(writer, finalName, member.Value);
                }
                else
                {
                    Serialize(writer, member, finalName: finalName, parentTypeName: parentTypeName);
                }
            }
        }

        private void Serialize(
            XmlWriter writer, 
            XmlSerializableMember member, 
            string finalName, 
            string parentTypeName,
            bool validateNestedType = true)
        {
            if (member.ValueType.IsPrimitive())
            {
                this.SerializeAsElement(writer, finalName, member.Value);
            }
            else if (member.HasXmlSerializableAttribute)
            {
                this.SerializeNestedObject(writer, member, name: finalName, parentTypeName: parentTypeName, validateNestedType);
            }
            else if (ReflectionUtility.IsList(member.ValueType, out Type innerType) && XmlDeserializer.CanDeserialize(innerType))
            {
                this.SerializeList(writer, member, innerType, name: finalName, parentTypeName: parentTypeName);
            }
            else
            {
                throw new XmlUnsupportedTypeException(member.ValueType, member.Name, source: parentTypeName);
            }
        }

        private void SerializeList(XmlWriter writer, XmlSerializableMember member, Type innerType, string name, string parentTypeName)
        {
            if (name.Contains("`"))
            {
                // since in some cases we use the type name as the tag name, we want to avoid that for lists
                // because for lists (or any collection), the type has a single quote in it that is considered illegal in xml names
                name = XmlTags.list;
            }

            dynamic list = member.Value;

            writer.WriteStartElement(name);
            foreach (var item in list)
            {
                string itemName = innerType.IsPrimitive() ? XmlTags.listItem : innerType.Name;
                var memberInfo = new XmlSerializableMember(itemName, item, null);
                Serialize(writer, memberInfo, finalName: memberInfo.Name, parentTypeName: parentTypeName);
            }
            writer.WriteEndElement();
        }

        private void SerializeNestedObject(
            XmlWriter writer,
            XmlSerializableMember member,
            string name, 
            string parentTypeName,
            bool validateNestedTypes = true)
        {
            if (validateNestedTypes)
            {
                if (nestedTypes.Contains(member.ValueType))
                {
                    throw new XmlInvalidException(
                        "circular dependency detected", 
                        member.ValueType, 
                        member.Name, 
                        source: parentTypeName);
                }

                nestedTypes.Add(member.ValueType);
            }
            
            Serialize(writer, member.Value, name);
            nestedTypes.Remove(member.ValueType);
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
