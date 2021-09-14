namespace Projapocsur.Entities
{
    using System;
    using System.Xml.Serialization;
    using Projapocsur.Common;
    using Projapocsur.Entities.Definitions;

    [Serializable]
    public class Stat : Entity
    {
        public float Value;

        public StatDef Def { get; private set; }
    }
}