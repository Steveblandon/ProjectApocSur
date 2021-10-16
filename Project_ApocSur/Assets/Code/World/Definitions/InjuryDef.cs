namespace Projapocsur.World
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class InjuryDef : Def
    {
        [XmlMember]
        public float BleedingRate { get; protected set; }

        [XmlMember]
        public float Pain { get; protected set; }

        [XmlMember]
        public float HealingRateMultiplier { get; protected set; } = 1f;
    }
}