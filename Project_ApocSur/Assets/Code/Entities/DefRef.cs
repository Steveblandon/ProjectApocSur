namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Xml.Serialization;
    using Projapocsur.Common;

    /// <summary>
    /// To avoid having Defs that fully define other defs they reference, this object should be used instead.
    /// It helps with making sure only one instance of each unique def exists in memory to avoid wasting space
    /// and helps with linking to those instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DefRef<T> where T : Def
    {
        [XmlAttribute]
        public string RefDefName;

        [XmlIgnore]
        public T Def { get; private set; }

        public void PostLoad()
        {
            DefinitionFinder.TryFind(RefDefName, out T def);
            this.Def = def;
        }
    }
}
