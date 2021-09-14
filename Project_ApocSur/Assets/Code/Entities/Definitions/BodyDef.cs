namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class BodyDef : Def
    {
        public float HeightBase;

        public float HeightRange;

        public float MaxHitPointsBase;

        public float BloodAmount;

        [XmlArray]
        [XmlArrayItem(ElementName = nameof(BodyPart))]
        public List<DefRef<BodyPartDef>> BodyParts = new List<DefRef<BodyPartDef>>();

        public override void PostLoad()
        {
            base.PostLoad();
            BodyParts.PostLoad();
        }
    }
}