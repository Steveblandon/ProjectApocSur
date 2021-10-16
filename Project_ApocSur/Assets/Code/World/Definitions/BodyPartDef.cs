namespace Projapocsur.World
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyPartDef : Def
    {
        [XmlMember]
        public int Size { get; protected set; }

        [XmlMember]
        public float BaseLength { get; protected set; }

        [XmlMember]
        public float BaseFloorOffset { get; protected set; }

        [XmlMember]
        public float MaxHitpoints { get; protected set; }

        [XmlMember]
        public float HealingRate { get; protected set; }
    }
}
