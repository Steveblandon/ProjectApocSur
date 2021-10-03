namespace Projapocsur.Things
{
    using System;
    using Projapocsur.Common.Serialization;
    using static Projapocsur.Things.SeverityConfig;

    [XmlSerializable]
    public class Injury : Thing<InjuryDef>
    {
        [XmlMember]
        private float healThreshold;

        [XmlMember]
        private float healedAmount;

        public Injury() { }

        public Injury(string defName, SeverityLevel severity) : base(defName)
        {
            float bleedingRate = this.Def.BleedingRate * SeverityAmpliflier[severity];
            float pain = this.Def.Pain * SeverityAmpliflier[severity];

            this.BleedingRate = new Stat(DefNameOf.Stat.BleedingRate, bleedingRate, bleedingRate);
            this.PainIncrease = new Stat(DefNameOf.Stat.PainIncrease, pain, pain);
            this.healThreshold = this.Def.HealThreshold;

            this.HealingRateMultiplier = this.Def.HealingRateMultiplier / SeverityAmpliflier[severity];
            this.Severity = severity;
        }

        [XmlMember]
        public SeverityLevel Severity { get; private set; }

        [XmlMember]
        public Stat BleedingRate { get; private set; }

        [XmlMember]
        public Stat PainIncrease { get; private set; }

        [XmlMember]
        public bool IsHealed { get; private set; }

        [XmlMember]
        public float HealingRateMultiplier { get; private set; }

        public void OnStart(InjuryProcessingContext context)
        {
            context.Pain += this.PainIncrease.Value;
        }

        public void OnUpdate(InjuryProcessingContext context)
        {
            float healingRate = context.HealingRate.Value * this.HealingRateMultiplier;

            if (healingRate > 0)
            {
                float newHealedAmount = this.healedAmount + healingRate;
                this.healedAmount = Math.Min(newHealedAmount, this.healThreshold);
                float healEffectMultiplier = 1f - (this.healedAmount / this.healThreshold);

                float previousPain = this.PainIncrease.Value;
                this.PainIncrease *= healEffectMultiplier;
                float painDiff = previousPain - this.PainIncrease.Value;

                this.BleedingRate *= healEffectMultiplier;
                
                context.Pain -= painDiff;
                context.BloodLoss += this.BleedingRate.Value;

                this.IsHealed = this.healedAmount == this.healThreshold;
            }
        }

        public void OnDestroy(InjuryProcessingContext context)
        {
            context.Pain -= this.PainIncrease.Value;
        }
    }
}