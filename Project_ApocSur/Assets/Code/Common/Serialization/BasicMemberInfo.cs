namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Reflection;

    public class BasicMemberInfo
    {
        public BasicMemberInfo(string name, object value, Type valueType)
        {
            this.Name = name;
            this.Value = value;
            this.ValueType = valueType;

            this.HasXmlSerializableAttribute = this.ValueType.GetCustomAttribute<XmlSerializableAttribute>() != null;
        }

        /// <summary>
        /// The name of the field/property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value of the field/property.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// The type of the field/property (not to be confused with <see cref="XmlSerializableMember"/>'s type).
        /// </summary>
        public Type ValueType { get; private set; }

        public bool HasXmlSerializableAttribute { get; private set; }
    }
}
