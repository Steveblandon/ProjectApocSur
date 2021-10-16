namespace Projapocsur.Common.Serialization
{
    using System;

    /// <summary>
    /// Indicates a field or property can be serialized by the <see cref="XmlSerializer"/>. It will be ignored if the class itself
    /// is not marked with the <see cref="XmlSerializableAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XmlMemberAttribute : Attribute
    {
        /// <summary>
        /// If not set, the serializer defaults to using the field/property name.
        /// </summary>
        public string PreferredName { get; protected set; }

        /// <summary>
        /// Indicates whether the member should be treated as a xml attribute instead of an element.
        /// </summary>
        public bool IsAttribute { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preferredName">If not set, the serializer defaults to using the field/property name.</param>
        /// <param name="isAttribute">Indicates whether the member should be serialized as a xml attribute instead of an element.</param>
        public XmlMemberAttribute(string preferredName = null, bool isAttribute = false)
        {
            this.PreferredName = preferredName;
            this.IsAttribute = isAttribute;
        }
    }
}
