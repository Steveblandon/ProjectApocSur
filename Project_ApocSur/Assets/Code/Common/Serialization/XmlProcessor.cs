namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Projapocsur.Common.Extensions;
    using Projapocsur.Common.Utilities;

    // TODO: split this up into XmlSerializer, XmlDeserializer, XmlSerializableMember and XmlSerializableMemberExtractor

    /// <summary>
    /// Custom XMLSerializer that unlike the .NET serializers allows for the serialization of any primitive field or property marked 
    /// with an attribute (<see cref="XmlMemberAttribute"/>), no matter what its access level is.
    /// Other than primitive types, the only complex types supported at the moment are custom ones marked with the
    /// <see cref="XmlSerializableAttribute"/>.
    /// </summary>
    public class XmlProcessor
    {
        private IReadOnlyDictionary<Type, Func<string, dynamic>> deserializationStrategies =
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

        private ISet<Type> typeTree = new HashSet<Type>();

        public void Serialize(Stream stream, object obj)
        {
            typeTree.Add(obj.GetType());

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                try
                {
                    Serialize(writer, obj);
                } catch(Exception ex)
                {
                    writer.Flush();
                    throw ex;
                }
            }
        }

        public void Serialize(XmlWriter writer, object obj, string rootName = null)
        {
            writer.WriteStartElement(rootName ?? obj.GetType().Name);

            if (obj.GetType().IsPrimitive())
            {
                writer.WriteValue(obj);
                writer.WriteEndElement();
                return;
            }

            if (obj.GetType().GetCustomAttribute<XmlSerializableAttribute>() == null)
            {
                throw new XmlUnsupportedTypeException(obj.GetType(), rootName ?? nameof(obj), nameof(Serialize));
            }

            LinkedList<XmlSerializableMember> serializableMembers = this.GetSerializableMembers(obj);
            
            foreach (var member in serializableMembers)
            {
                if (member.HasXmlSerializableAttribute)
                {
                    if (typeTree.Contains(member.ValueType))
                    {
                        throw new XmlInvalidException("circular dependency detected", member.ValueType, member.Name, source: obj.GetType().Name);
                    }

                    typeTree.Add(member.ValueType);
                    Serialize(writer, member.Value, member.XmlMemberAttribute.PreferredName ?? member.Name);
                }
                else if (member.ValueType.IsPrimitive())
                {
                    if (member.XmlMemberAttribute.IsAttribute)
                    {
                        writer.WriteStartAttribute(member.XmlMemberAttribute.PreferredName ?? member.Name);
                        writer.WriteValue(member.Value);
                        writer.WriteEndAttribute();
                    }
                    else
                    {
                        writer.WriteStartElement(member.XmlMemberAttribute.PreferredName ?? member.Name);
                        writer.WriteValue(member.Value);
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    throw new XmlUnsupportedTypeException(obj.GetType(), member.Name, obj.GetType().Name);
                }
            }

            writer.WriteEndElement();
        }

        public void Deserialize<T>(Stream stream, out T data) where T : new()
        {
            data = new T();

            if (data.GetType().GetCustomAttribute<XmlSerializableAttribute>() == null)
            {
                throw new XmlUnsupportedTypeException(data.GetType(), nameof(data), nameof(Deserialize));
            }

            using (XmlReader reader = XmlReader.Create(stream))
            {
                Deserialize(reader, data);
            }
        }

        public void Deserialize(XmlReader reader, object data)
        {
            string currentElementName = string.Empty;
            string parentElementName = null;
            var memberByName = new Dictionary<string, XmlSerializableMember>();
            IEnumerable<XmlSerializableMember> serializableMembers = this.GetSerializableMembers(data);

            serializableMembers.ForEach((member) => memberByName[member.XmlMemberAttribute.PreferredName ?? member.Name] = member);

            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        LogUtility.Log(LogLevel.Info, $"element: <{reader.Name}>");
                        currentElementName = reader.Name;
                        parentElementName ??= reader.Name;

                        if (currentElementName != parentElementName
                            && memberByName.TryGetValue(reader.Name, out XmlSerializableMember member)
                            && (!deserializationStrategies.ContainsKey(member.ValueType)))
                        {
                            if (!member.HasXmlSerializableAttribute)
                            {
                                throw new XmlUnsupportedTypeException(data.GetType(), nameof(data), nameof(Deserialize));
                            }

                            Deserialize(reader, member.Value);
                            break;
                        }

                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                LogUtility.Log(LogLevel.Info, $"attribute: {reader.Name}={reader.Value}, type={reader.ValueType}");
                                DeserializeNode(reader.Name, reader.Value, data.GetType(), memberByName);
                            }
                        }

                        break;
                    case XmlNodeType.Text:
                        LogUtility.Log(LogLevel.Info, $"value: {reader.Value}, name={currentElementName}");
                        DeserializeNode(currentElementName, reader.Value, data.GetType(), memberByName);
                        break;
                    case XmlNodeType.EndElement:
                        LogUtility.Log(LogLevel.Info, $"element end: </ {reader.Name}");
                        if (reader.Name == parentElementName)
                        {
                            return;
                        }
                        break;
                }
            } while (reader.Read());
        }

        private void DeserializeNode(
            string name, 
            string value, 
            Type parentType, 
            IReadOnlyDictionary<string, XmlSerializableMember> memberByNameLookup)
        {
            if (memberByNameLookup.TryGetValue(name, out XmlSerializableMember member))
            {
                if (deserializationStrategies.TryGetValue(member.ValueType, out Func<string, dynamic> deserializeString))
                {
                    member.Value = deserializeString(value);
                }
                else
                {
                    throw new XmlUnsupportedTypeException(member.ValueType, member.Name, parentType.Name);
                }
            }
            else
            {
                throw new XmlInvalidException("no matching field/property found", null, name, source: parentType.Name);
            }
        }

        private LinkedList<XmlSerializableMember> GetSerializableMembers(object parent)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var serializableMembers = new LinkedList<XmlSerializableMember>();            

            FieldInfo[] fields = parent.GetType().GetFields(bindingFlags);
            PropertyInfo[] properties = parent.GetType().GetProperties(bindingFlags);

            this.GetSerializableMembers(fields, parent, serializableMembers);
            this.GetSerializableMembers(properties, parent, serializableMembers);

            return serializableMembers;
        }

        private void GetSerializableMembers(
            FieldInfo[] infoItems,
            object parent,
            LinkedList<XmlSerializableMember> validMembers)
        {
            foreach (var infoItem in infoItems)
            {
                XmlMemberAttribute attributeInfo = infoItem.GetCustomAttribute<XmlMemberAttribute>();

                if (attributeInfo != null)
                {
                    XmlSerializableMember member = new XmlSerializableMember(infoItem, parent, attributeInfo);
                    this.ValidateMember(member, validMembers);
                }
            }
        }

        private void GetSerializableMembers(
            PropertyInfo[] infoItems, 
            object parent, 
            LinkedList<XmlSerializableMember> validMembers)
        {
            foreach(var infoItem in infoItems)
            {
                XmlMemberAttribute attributeInfo = infoItem.GetCustomAttribute<XmlMemberAttribute>();

                if (attributeInfo != null)
                {
                    XmlSerializableMember member = new XmlSerializableMember(infoItem, parent, attributeInfo);
                    this.ValidateMember(member, validMembers);
                }
            }
        }

        private void ValidateMember(
            XmlSerializableMember member,
            LinkedList<XmlSerializableMember> validMembers)
        {
            if (member.XmlMemberAttribute.IsAttribute && !member.ValueType.IsPrimitive())
            {
                throw new XmlInvalidException("attribute must be a primitive", member.ValueType, member.Name);
            }

            if (member.XmlMemberAttribute.IsAttribute)  // attributes need to be processed first otherwise the writer will flip out.
            {
                validMembers.AddFirst(member);
            }
            else
            {
                validMembers.AddLast(member);
            }
        }

        private class XmlSerializableMember
        {
            private Action<object, object> setValue;
            private Func<object, object> getValue;
            private object parent;

            private XmlSerializableMember(object parent, XmlMemberAttribute xmlMemberAttribute)
            {
                this.parent = parent;
                this.XmlMemberAttribute = xmlMemberAttribute;
            }

            public XmlSerializableMember(
                PropertyInfo propertyInfo, 
                object parent, 
                XmlMemberAttribute xmlMemberAttribute = null) : this(parent, xmlMemberAttribute)
            {
                this.setValue = propertyInfo.SetValue;
                this.getValue = propertyInfo.GetValue;
                this.Name = propertyInfo.Name;
                this.PostConstruct();
            }

            public XmlSerializableMember(
                FieldInfo fieldInfo, 
                object parent, 
                XmlMemberAttribute xmlMemberAttribute = null) : this(parent, xmlMemberAttribute)
            {
                this.setValue = fieldInfo.SetValue;
                this.getValue = fieldInfo.GetValue;
                this.Name = fieldInfo.Name;
                this.PostConstruct();
            }

            public string Name { get; private set; }

            public object Value
            {
                get => getValue(parent);
                set => setValue(parent, value);
            }

            public Type ValueType { get; private set; }

            public XmlMemberAttribute XmlMemberAttribute { get; private set; }

            public bool HasXmlSerializableAttribute { get; private set; }

            private void PostConstruct()
            {
                if (this.Value == null)
                {
                    throw new XmlInvalidException("cannot be null", null, this.Name, "null", source: this.parent.GetType().Name);
                }

                this.ValueType = this.Value.GetType();
                this.HasXmlSerializableAttribute = this.ValueType.GetCustomAttribute<XmlSerializableAttribute>() != null;
            }
        }
    }
}
