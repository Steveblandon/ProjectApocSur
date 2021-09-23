namespace Projapocsur.Things
{
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;

    public abstract class Thing<T> : ILoadable where T : Def
    {
        [XmlMember]
        private DefRef<T> defRef;

        public Thing() { }

        public Thing(string defRefName)
        {
            this.defRef = new DefRef<T>(defRefName);
            this.defRef.LinkToDef();
        }

        public T Def { get => this.defRef?.Def; }

        public virtual void PostLoad() 
        {
            this.defRef?.LinkToDef();
        }
    }
}
