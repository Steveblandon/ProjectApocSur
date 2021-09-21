namespace Projapocsur.Entities
{
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Entities.Definitions;

    public abstract class Entity : ILoadable
    {
        [XmlMember]
        private DefRef<Def> defRef;

        public Entity() { }

        public Entity(string defRefName)
        {
            this.defRef = new DefRef<Def>(defRefName);
        }

        public Def Def { get; private set; }

        public virtual void PostLoad() 
        {
            this.defRef?.LinkToDef();
        }
    }
}
