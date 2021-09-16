namespace Projapocsur.CustomAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XmlMemberAttribute : Attribute
    {
        public string PreferredName { get; private set; }

        public bool IsAttribute { get; private set; }

        public XmlMemberAttribute(string preferredName = null, bool isAttribute = false)
        {
            this.PreferredName = preferredName;
            this.IsAttribute = isAttribute;
        }
    }
}
