﻿namespace Projapocsur.Entities
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Entities.Definitions;

    [XmlSerializable]
    public class Body : Entity
    {
        [XmlMember]
        public Stat Height { get; private set; }

        [XmlMember]
        public Stat HitPoints { get; private set; }

        [XmlMember]
        public Stat BloddLoss { get; private set; }

        [XmlMember]
        public Stat HealingRate { get; private set; }

        [XmlMember]
        public List<BodyPart> BodyParts { get; private set; }

        [XmlMember]
        public Stance CurrentStance { get; private set; }

        private Dictionary<Stance, List<BodyHitProcessor>> hitProcessors;

        public Body() { }

        public Body(BodyDef def) : base (def.Name)
        {
            // instantiate class using def
        }

        public void OnUpdate(InjuryProcessingContext context)
        {

        }

        public void TakeDamage(BodyHitInfo hit)
        {

        }

        private class BodyHitProcessor
        {
        }
    }

    public enum Stance
    {
        Standing,
        Crouch,
        Sitting,
        Prone,
    }
}