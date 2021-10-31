namespace Projapocsur.World
{
    using Projapocsur.Serialization;

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
        public bool IsVital { get; protected set; }
    }
}
