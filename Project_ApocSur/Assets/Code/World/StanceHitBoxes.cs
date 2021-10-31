namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class StanceHitBoxes
    {
        public StanceHitBoxes() { }

        public StanceHitBoxes(DefRef<StanceDef> defRef, List<BodyHitBox> hitBoxes)
        {
            this.StanceDefRef = defRef;
            this.HitBoxes = hitBoxes;
        }

        [XmlMember]
        public DefRef<StanceDef> StanceDefRef { get; protected set; }

        [XmlMember]
        public List<BodyHitBox> HitBoxes { get; protected set; }
    }
}