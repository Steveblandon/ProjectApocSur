namespace Projapocsur.Entities.Definitions
{
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class StatDef : Def
    {
        [XmlMember]
        public string Format { get; private set; }
    }
}