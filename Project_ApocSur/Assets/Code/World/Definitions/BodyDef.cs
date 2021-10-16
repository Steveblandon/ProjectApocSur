namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyDef : Def
    {
        [XmlMember]
        public float MaxHeightDeviationFactor { get; private set; }

        [XmlMember]
        public float MaxHitPoints { get; private set; }

        [XmlMember]
        public float BaseHealingRate { get; private set; }

        [XmlMember]
        public float PainThreshold { get; private set; }

        [XmlMember]
        public int HitBoxCount { get; private set; }

        [XmlMember]
        public List<DefRef<BodyPartDef>> BodyParts { get; set; }

        [XmlMember]
        public List<DefRef<StanceDef>> StanceCapabilities { get; private set; }

        [XmlMember]
        public DefRef<StanceDef> DefaultStance { get; private set; }

        public override void PostLoad()
        {
            base.PostLoad();
        }
    }
}