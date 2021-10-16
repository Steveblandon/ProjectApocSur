namespace Projapocsur.World
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class InjuryDef : Def
    {
        [XmlMember]
        public float BleedingRate { get; private set; }

        [XmlMember]
        public float Pain { get; private set; }

        [XmlMember]
        public float HealingRateMultiplier { get; private set; } = 1f;
    }
}