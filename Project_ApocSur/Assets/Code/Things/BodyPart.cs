namespace Projapocsur.Things
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class BodyPart : Thing<BodyPartDef>
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