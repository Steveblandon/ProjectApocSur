namespace Projapocsur.Things
{
    using Projapocsur.Common.Serialization;

    public enum SeverityLevel
    {
        Minor,
        Major,
        Severe
    }

    [XmlSerializable]
    public class Injury : Thing
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
            this.defRefInternal?.LinkToDef();
        }
    }
}