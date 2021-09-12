﻿namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public abstract class Def : Entity
    {
        [XmlAttribute]
        public string Name;

        public string Label;

        public string Description;
    }
}