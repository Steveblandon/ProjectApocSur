namespace Projapocsur.Entities
{
    using System.Xml.Serialization;
    using Projapocsur.Common;
    using Projapocsur.Entities.Definitions;

    public class BodyPart : Entity
    {
        [XmlAttribute]
        public string RefDefName;

        public BodyPartDef Def { get; private set; }

        public Stat HitPoints { get; private set; }

        public override void PostLoad()
        {
            base.PostLoad();

            DefinitionFinder.TryFind(RefDefName, out BodyPartDef def);
            this.Def = def;
        }
    }
}