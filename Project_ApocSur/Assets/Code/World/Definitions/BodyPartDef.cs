namespace Projapocsur.World
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyPartDef : Def
    {
        [XmlMember]
        public int Size { get; private set; }

        [XmlMember]
        public float BaseLength { get; private set; }

        [XmlMember]
        public float BaseFloorOffset { get; private set; }

        [XmlMember]
        public float MaxHitpoints { get; private set; }

        [XmlMember]
        public float HealingRate { get; private set; }
    }
}
