namespace Projapocsur.Entities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Body : Entity
    {
        public Stat Height { get; private set; }

        public Stat HitPoints { get; private set; }

        public Stat MaxHitPoints { get; private set; }

        public Stat BloddLoss { get; private set; }

        public List<BodyPart> bodyParts;
    }
}