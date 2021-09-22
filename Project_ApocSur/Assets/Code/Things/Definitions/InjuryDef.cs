namespace Projapocsur.Things
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
        public float HealThreshold { get; private set; }
    }
}