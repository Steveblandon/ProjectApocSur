namespace Projapocsur.World
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
        }

        public T Def { get => this.defRef?.Def; }

        public virtual void PostLoad() { }
    }
}
