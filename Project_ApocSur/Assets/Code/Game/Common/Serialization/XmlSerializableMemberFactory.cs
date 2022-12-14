namespace Projapocsur.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    

    /// <summary>
    /// Determines what fields/properties of an instance are serializable and returns them as <see cref="XmlSerializableMember"/>.
    /// </summary>
    public class XmlSerializableMemberFactory
    {
        private static BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private object parent;
        private LinkedList<XmlSerializableMember> members;

        private XmlSerializableMemberFactory(object obj)
        {
            this.parent = obj;
            this.members = new LinkedList<XmlSerializableMember>();
        }

        public static LinkedList<XmlSerializableMember> GetSerializableMembers(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags);
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags);

            var factory = new XmlSerializableMemberFactory(obj);

            factory.Process(fields, (field) => field);
            factory.Process(properties, (prop) => prop);

            return factory.members;
        }

        private void Process<T>(T[] infoItems, Func<T, dynamic> value) where T : MemberInfo
        {
            foreach (var infoItem in infoItems)
            {
                XmlMemberAttribute attributeInfo = infoItem.GetCustomAttribute<XmlMemberAttribute>();

                if (attributeInfo != null)
                {
                    XmlSerializableMember member = new XmlSerializableMember(value(infoItem), this.parent, attributeInfo);
                    this.AddMember(member);
                }
            }
        }

        private void AddMember(XmlSerializableMember member)
        {
            if (member.XmlMemberAttribute.IsAttribute && !member.ValueType.IsPrimitive())
            {
                throw new XmlInvalidException("attribute can not be a complex type", member.ValueType, paramName: member.Name, source: this.parent.GetType().Name);
            }

            if (member.XmlMemberAttribute.IsAttribute)  // attributes need to be processed first otherwise the XmlWriter will flip out.
            {
                this.members.AddFirst(member);
            }
            else
            {
                this.members.AddLast(member);
            }
        }
    }
}
