namespace Projapocsur.Common.Serialization
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Contains all the information needed of a serializable class's member for the <see cref="XmlSerializer"/> to work with.
    /// </summary>
    public class XmlSerializableMember : BasicMemberInfo
    {
        private Action<object, object> setValue;
        private Func<object, object> getValue;
        private object parent;

        private XmlSerializableMember(string name, object value, object parent, XmlMemberAttribute xmlMemberAttribute)
            : base (name, value, value?.GetType())
        {
            this.parent = parent;
            this.XmlMemberAttribute = xmlMemberAttribute;
        }

        public XmlSerializableMember(PropertyInfo propertyInfo, object parent, XmlMemberAttribute xmlMemberAttribute) 
            : this(propertyInfo.Name, propertyInfo.GetValue(parent), parent, xmlMemberAttribute)
        {
            this.setValue = propertyInfo.SetValue;
            this.getValue = propertyInfo.GetValue;
            this.PostConstruct();
        }

        public XmlSerializableMember(FieldInfo fieldInfo, object parent, XmlMemberAttribute xmlMemberAttribute)
            : this(fieldInfo.Name, fieldInfo.GetValue(parent), parent, xmlMemberAttribute)
        {
            this.setValue = fieldInfo.SetValue;
            this.getValue = fieldInfo.GetValue;
            this.PostConstruct();
        }

        public new object Value
        {
            get => getValue(parent);
            set => setValue(parent, value);
        }

        public XmlMemberAttribute XmlMemberAttribute { get; private set; }

        private void PostConstruct()
        {
            if (this.Value == null)
            {
                throw new XmlInvalidException("cannot be null", null, this.Name, "null", source: this.parent.GetType().Name);
            }
        }
    }
}
