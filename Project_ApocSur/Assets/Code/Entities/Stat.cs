namespace Projapocsur.Entities
{
    using System;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Entities.Definitions;

    [XmlSerializable]
    public class Stat : Entity
    {
        public event Action OnStateChangeEvent;

        [XmlMember]
        public float Value { get; private set; }

        [XmlMember]
        public float MinValue { get; private set; }

        [XmlMember]
        public float MaxValue { get; private set; }

        public Stat() { }

        public Stat(StatDef def) : base(def.Name)
        {
            // instantiate class using def
        }

    }
}