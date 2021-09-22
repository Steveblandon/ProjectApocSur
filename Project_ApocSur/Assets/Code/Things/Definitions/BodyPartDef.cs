namespace Projapocsur.Things
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyPartDef : Def
    {
        [XmlMember]
        public int Size { get; private set; }

        [XmlMember]
        public float Height { get; private set; }

        [XmlMember]
        public float FloorOffset { get; private set; }

        [XmlMember]
        public float MaxHitpoints { get; private set; }

        [XmlMember]
        public float HealingRate { get; private set; }
    }
}
