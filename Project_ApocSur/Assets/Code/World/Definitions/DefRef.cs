namespace Projapocsur.World
{
    using Projapocsur.Serialization;

    /// <summary>
    /// <para>
    /// this helps with linking between defs to avoid having Defs that fully define other referenced defs when serialized.
    /// That way only one instance of each unique def exists in memory and storage to avoid wasting space.
    /// </para>
    /// Additionally, the <see cref="RefDefName"/> property facilitates refactoring def names with tooling by having a consistent
    /// prefix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// NOTICE: Don't use this as a key in a hash table, it won't work. Use the DefRefName instead.
    /// </remarks>
    [XmlSerializable]
    public class DefRef<T> where T : Def
    {
        private T defInternal;

        public DefRef() { }

        public DefRef(string refDefName)
        {
            this.RefDefName = refDefName;
        }

        [XmlMember(isAttribute:true)]
        public string RefDefName { get; protected set; }

        public T Def 
        {
            get 
            {
                if (this.defInternal == null)
                {
                    this.defInternal = DefinitionFinder.Find<T>(this.RefDefName);
                }

                return this.defInternal;
            } 
        }
    }
}
