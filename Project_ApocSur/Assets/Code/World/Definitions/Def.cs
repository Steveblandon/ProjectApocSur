namespace Projapocsur.World
{
    using Projapocsur.Serialization;

    // NOTE: the properties of this class have a protected-access setter instead of private so that derive classes
    // can serialize these values. Otherwise they don't have access to the setter and serialization will fail.

    public abstract class Def : ILoadable
    {
        [XmlMember(isAttribute:true)]
        public string Name { get; protected set; }

        [XmlMember]
        public string Label { get; protected set; }

        [XmlMember]
        public string Description { get; protected set; }

        public virtual void PostLoad() { }
    }
}