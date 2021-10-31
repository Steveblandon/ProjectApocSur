namespace Projapocsur.World
{
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class StanceDef : Def
    {
        [XmlMember]
        public float HeightMultiplier { get; protected set; }
    }
}