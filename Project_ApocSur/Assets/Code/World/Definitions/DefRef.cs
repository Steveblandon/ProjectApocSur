namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;
    using Projapocsur;

    /// <summary>
    /// <para>
    /// this helps with linking between defs to avoid having Defs that fully define other referenced defs when serialized.
    /// That way only one instance of each unique def exists in memory and storage to avoid wasting space.
    /// </para>
    /// Additionally, the <see cref="RefDefName"/> property facilitates refactoring def names with tooling by having a consistent
    /// prefix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlSerializable]
    public class DefRef<T> where T : Def
    {
        public DefRef() { }

        public DefRef(string refDefName)
        {
            this.RefDefName = refDefName;
        }

        [XmlMember(isAttribute:true)]
        public string RefDefName { get; private set; }

        public T Def { get; private set; }

        public void LinkToDef()
        {
            this.Def = DefinitionFinder.Find<T>(this.RefDefName);
        }
    }

    public static class DefRefExtensions
    {
        public static void LinkDef<T>(this List<DefRef<T>> defRefs) where T : Def => defRefs.ForEach((defRef) => defRef.LinkToDef());
    }
}
