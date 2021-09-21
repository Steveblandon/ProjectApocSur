namespace Projapocsur.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Entities.Definitions;

    public enum SeverityLevel
    {
        Minor,
        Major,
        Severe
    }

    [XmlSerializable]
    public class Injury : Entity
    {
        [XmlMember]
        public float BleedingRate { get; private set; }

        [XmlMember]
        public float Pain { get; private set; }

        [XmlMember]
        private float healThreshold;

        [XmlMember]
        private float healedAmount;

        [XmlMember]
        private bool isHealed;

        private DefRef<InjuryDef> defRefInternal;

        public Injury() { }

        public Injury(InjuryDef def, SeverityLevel severity) : base(def.Name)
        {
            // instantiate class using def
            // adjust bleeding rate and pain based on severity
        }

        public override void PostLoad()
        {
            base.PostLoad();
            defRefInternal?.LinkToDef();
        }
    }
}