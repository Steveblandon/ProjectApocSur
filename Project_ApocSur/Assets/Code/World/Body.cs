﻿namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class Body : Thing<BodyDef>
    {
        [XmlMember]
        public Stat Height { get; private set; }

        [XmlMember]
        public Stat HitPoints { get; private set; }

        [XmlMember]
        public Stat BloodLoss { get; private set; }

        [XmlMember]
        public Stat HealingRate { get; private set; }

        [XmlMember]
        public List<BodyPart> BodyParts { get; private set; }

        [XmlMember]
        public Stance CurrentStance { get; private set; }

        [XmlMember]
        private Dictionary<Stance, List<BodyHitBox>> hitBoxes;

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
    }

    public enum Stance
    {
        Standing,
        Crouch,
        Sitting,
        Prone,
    }
}