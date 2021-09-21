namespace Projapocsur.Entities.Definitions
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Core;

    /// <summary>
    /// To avoid having Defs that fully define other defs they reference, this object should be used instead.
    /// It helps with making sure only one instance of each unique def exists in memory to avoid wasting space
    /// and helps with linking to those instances.
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
            DefinitionFinder.TryFind(this.RefDefName, out T def);
            this.Def = def;
        }
    }

    public static class DefRefExtensions
    {
        public static void LinkDef<T>(this List<DefRef<T>> defRefs) where T : Def => defRefs.ForEach((defRef) => defRef.LinkToDef());
    }
}
