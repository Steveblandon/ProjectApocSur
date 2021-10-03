namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyDef : Def
    {
        [XmlMember]
        public float Height { get; private set; }

        [XmlMember]
        public float HeightRange { get; private set; }

        [XmlMember]
        public float MaxHitPoints { get; private set; }

        [XmlMember]
        public List<DefRef<BodyPartDef>> BodyParts { get;  set; }

        public override void PostLoad()
        {
            base.PostLoad();
            this.BodyParts.LinkDef();
        }
    }
}