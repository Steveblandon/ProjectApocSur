namespace Projapocsur.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;    

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
            this.nestedTypes = new HashSet<Type>();
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

            writer.WriteStartElement(this.XmlValidName(currentNodeName));
            this.SerializeMembers(writer, objTypeName, serializableMembers);
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
                    this.Serialize(writer, member, finalName: finalName, parentTypeName: parentTypeName);
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
            dynamic list = member.Value;

            writer.WriteStartElement(this.XmlValidName(name));
            foreach (var item in list)
            {
                string itemName = innerType.IsPrimitive() ? XmlTags.listItem : innerType.Name;
                var memberInfo = new XmlSerializableMember(itemName, item, null);
                this.Serialize(writer, memberInfo, finalName: memberInfo.Name, parentTypeName: parentTypeName);
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
                if (this.nestedTypes.Contains(member.ValueType))
                {
                    throw new XmlInvalidException(
                        "circular dependency detected", 
                        member.ValueType, 
                        member.Name, 
                        source: parentTypeName);
                }

                this.nestedTypes.Add(member.ValueType);
            }

            this.Serialize(writer, member.Value, name);
            this.nestedTypes.Remove(member.ValueType);
        }

        private void SerializeAsElement(XmlWriter writer, string name, object value)
        {
            writer.WriteStartElement(this.XmlValidName(name));
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        private void SerializeAsAttribute(XmlWriter writer, string name, object value)
        {
            writer.WriteStartAttribute(this.XmlValidName(name));
            writer.WriteValue(value);
            writer.WriteEndAttribute();
        }

        /// <summary>
        /// Since in some cases we use the type name as the tag name, we need to make sure it's a valid xml name.
        /// In the case of generics for example there is an invalid character that must be omitted.
        /// </summary>
        /// <param name="name"> The string to parse.</param>
        /// <returns>The scrubbed string or original string.</returns>
        private string XmlValidName(string name)
        {
            int foundInvalidChar = name.IndexOf('`');

            if (foundInvalidChar > 0)
            {
                return name.Substring(0, foundInvalidChar);
            }

            return name;
        }
    }
}
