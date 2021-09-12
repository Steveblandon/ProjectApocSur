namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class StatModifierDefRef : DefRef<StatModifierDef>
    {
        [XmlAttribute]
        public float Change;

        [XmlAttribute]
        public float Multiplier = 1;
    }
}