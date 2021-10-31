namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class BodyDef : Def
    {
        [XmlMember]
        public float MaxHeightDeviationFactor { get; protected set; }

        [XmlMember]
        public float BaseHealingRate { get; protected set; }

        [XmlMember]
        public float PainThreshold { get; protected set; }

        [XmlMember]
        public int HitBoxCount { get; protected set; }

        [XmlMember]
        public List<DefRef<BodyPartDef>> BodyParts { get; set; }

        [XmlMember]
        public List<DefRef<StanceDef>> StanceCapabilities { get; protected set; }

        [XmlMember]
        public DefRef<StanceDef> DefaultStance { get; protected set; }

        public override void PostLoad()
        {
            base.PostLoad();
        }
    }
}