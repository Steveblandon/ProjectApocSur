namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot]
    public class StatDef : Def
    {
        public string Description;
    }
}