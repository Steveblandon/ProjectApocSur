namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Reflection;
    using Projapocsur.Common;

    /// <summary>
    /// Contains all the information needed of a serializable class's member for the <see cref="XmlSerializer"/> to work with.
    /// </summary>
    public class XmlSerializableMember
    {
        private object defaultValue;
        private Action<object, object> setValue;
        private Func<object, object> getValue;
        private object parent;
        private Type memberType;

        private XmlSerializableMember(string name, Type refType, object parent, XmlMemberAttribute xmlMemberAttribute)
        {
            this.memberType = refType;
            this.Name = name;
            this.parent = parent;
            this.XmlMemberAttribute = xmlMemberAttribute ?? new XmlMemberAttribute();
        }

        public XmlSerializableMember(string name, object value, object parent)
            : this(name, value.GetType(), parent, null)
        {
            this.UseDefaultValue(value);
            this.PostConstruct();
        }

        public XmlSerializableMember(string name, Type type, object parent)
            : this(name, type, parent, null)
        {
            object value = this.AttemptToInstantiateValueFromType(type);
            this.UseDefaultValue(value);
            this.PostConstruct();
        }

        public XmlSerializableMember(PropertyInfo propertyInfo, object parent, XmlMemberAttribute xmlMemberAttribute = null) 
            : this(propertyInfo.Name, propertyInfo.PropertyType, parent, xmlMemberAttribute)
        {
            this.setValue = propertyInfo.SetValue;
            this.getValue = propertyInfo.GetValue;
            this.PostConstruct();
        }

        public XmlSerializableMember(FieldInfo fieldInfo, object parent, XmlMemberAttribute xmlMemberAttribute = null) 
            : this(fieldInfo.Name, fieldInfo.FieldType, parent, xmlMemberAttribute)
        {
            this.setValue = fieldInfo.SetValue;
            this.getValue = fieldInfo.GetValue;
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
            get => this.getValue(this.parent);
            set => this.setValue(this.parent, value);
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
                this.Value = this.AttemptToInstantiateValueFromType(this.memberType);
            }

            this.ValueType = this.Value.GetType();
            this.HasXmlSerializableAttribute = this.ValueType.GetCustomAttribute<XmlSerializableAttribute>() != null;
        }

        private object AttemptToInstantiateValueFromType(Type type)
        {
            object value = null;
            bool throwException = false;
            MissingMethodException ex = null;

            if (type == typeof(string))
            {
                value = string.Empty;
            }
            else if (!type.IsAbstract && !type.IsInterface)
            {
                ExceptionUtility.TryCatch(
                    () => value = Activator.CreateInstance(type),
                    out ex);
                throwException = ex != null;
            }
            else
            {
                throwException = true;
            }

            if (throwException)
            {
                throw new XmlInvalidException("reference type must be instantiatable if value is null", type, this.Name, "null", this.parent.GetType().Name, ex);
            }

            return value;
        }

        private void UseDefaultValue(object value)
        {
            this.defaultValue = value;
            this.setValue = (parent, val) => this.defaultValue = val;
            this.getValue = (parent) => this.defaultValue;
            this.memberType = value.GetType();
        }
    }
}
