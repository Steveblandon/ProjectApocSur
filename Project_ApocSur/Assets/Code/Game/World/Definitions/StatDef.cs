namespace Projapocsur.World
{
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class StatDef : Def
    {
        [XmlMember]
        public string Format { get; protected set; }
    }
}