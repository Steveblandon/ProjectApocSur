namespace Projapocsur.World
{
    using System;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class Injury : Thing<InjuryDef>
    {
        [XmlMember]
        private float healedAmount;

        public Injury() { }

        public Injury(string defName, SeverityLevel severity) : base(defName)
        {
            float bleedingRate = this.Def.BleedingRate * Config.SeverityAmpliflier[severity];
            float pain = this.Def.Pain * Config.SeverityAmpliflier[severity];

            this.BleedingRate = new Stat(DefNameOf.Stat.BleedingRate, bleedingRate, bleedingRate);
            this.PainIncrease = new Stat(DefNameOf.Stat.PainIncrease, pain, pain);

            this.HealingRateMultiplier = this.Def.HealingRateMultiplier / Config.SeverityAmpliflier[severity];
            this.Severity = severity;
        }

        [XmlMember]
        public SeverityLevel Severity { get; protected set; }

        [XmlMember]
        public Stat BleedingRate { get; protected set; }

        [XmlMember]
        public Stat PainIncrease { get; protected set; }

        [XmlMember]
        public bool IsHealed { get; protected set; }

        [XmlMember]
        public float HealingRateMultiplier { get; protected set; }

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
                this.healedAmount = Math.Min(newHealedAmount, Config.DefaultInjuryHealThreshold);
                float healEffectMultiplier = 1f - (this.healedAmount / Config.DefaultInjuryHealThreshold);

                float previousPain = this.PainIncrease.Value;
                this.PainIncrease *= healEffectMultiplier;
                float painDiff = previousPain - this.PainIncrease.Value;

                this.BleedingRate *= healEffectMultiplier;
                
                context.Pain -= painDiff;
                context.BloodLoss += this.BleedingRate.Value;

                this.IsHealed = this.healedAmount == Config.DefaultInjuryHealThreshold;
            }
        }

        public void OnDestroy(InjuryProcessingContext context)
        {
            context.Pain -= this.PainIncrease.Value;
        }
    }
}