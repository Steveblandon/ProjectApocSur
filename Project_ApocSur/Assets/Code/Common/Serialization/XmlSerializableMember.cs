namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Contains all the information needed of a serializable class's member for the <see cref="XmlSerializer"/> to work with.
    /// </summary>
    public class XmlSerializableMember
    {
        private object defaultValue;
        private Action<object, object> setValue;
        private Func<object, object> getValue;
        private object parent;

        private XmlSerializableMember(object parent, XmlMemberAttribute xmlMemberAttribute)
        {
            this.parent = parent;
            this.XmlMemberAttribute = xmlMemberAttribute ?? new XmlMemberAttribute();
        }

        public XmlSerializableMember(string name, object value, object parent)
            : this(parent, null)
        {
            this.Name = name;
            this.defaultValue = value;
            this.setValue = (parent, val) => this.defaultValue = val;
            this.getValue = (parent) => this.defaultValue;
            this.PostConstruct();
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

        /// <summary>
        /// The name of the field/property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value of the field/property.
        /// </summary>
        public object Value
        {
            get => getValue(parent);
            set => setValue(parent, value);
        }

        /// <summary>
        /// The type of the field/property (not to be confused with <see cref="XmlSerializableMember"/>'s type).
        /// </summary>
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
