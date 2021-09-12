namespace Projapocsur.Entities
{
    using System;
    using System.Xml.Serialization;
    using Projapocsur.Common;
    using Projapocsur.Entities.Definitions;

    [Serializable]
    public class StatModifier : Entity
    {
        [XmlAttribute]
        public string RefDefName;

        [XmlText]
        public float Value;

        public StatModifierDef Def { get; private set; }

        public override void PostLoad()
        {
            base.PostLoad();

            DefinitionFinder.TryFind(RefDefName, out StatModifierDef def);
            this.Def = def;
        }
    }
}