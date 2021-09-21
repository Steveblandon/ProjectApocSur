namespace Projapocsur.Entities
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Entities.Definitions;

    [XmlSerializable]
    public class BodyPart : Entity
    {
        [XmlMember]
        public Stat HitPoints { get; private set; }

        [XmlMember]
        public LinkedList<Injury> Injuries { get; private set; }

        public BodyPart() { }

        public BodyPart(BodyPartDef def) : base(def.Name)
        {
            // instantiate class using def
        }

        public void OnUpdate(InjuryProcessingContext context)
        {

        }
    }
}