namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot]
    public abstract class Def
    {
        [XmlAttribute]
        public string Name;
    }
}