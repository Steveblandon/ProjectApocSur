namespace Projapocsur.Things
{
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;

    public abstract class Thing : ILoadable
    {
        [XmlMember]
        private DefRef<Def> defRef;

        public Thing() { }

        public Thing(string defRefName)
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
