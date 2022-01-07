namespace Projapocsur.World
{
    using System;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class Injury : Thing<InjuryDef>
    {
        public event Action IsHealedEvent;

        public Injury() { }

        public Injury(string defName, SeverityLevel severity) : base(defName)
        {
            float bleedingRate = this.Def.BleedingRate * Config.SeverityAmpliflier[severity];
            this.BleedingRate = new Stat(DefNameOf.Stat.BleedingRate, bleedingRate, bleedingRate);
            this.IsBleedingSource = bleedingRate > 0f;
            
            float pain = this.Def.Pain * Config.SeverityAmpliflier[severity];
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
        public bool IsBleeding { get; protected set; }

        [XmlMember]
        public bool IsBleedingSource { get; protected set; }

        [XmlMember]
        public float HealedAmount { get; protected set; }

        [XmlMember]
        public float HealingRateMultiplier { get; protected set; }

        public void OnStart(InjuryProcessingContext context)
        {
            context.Pain += this.PainIncrease.Value;
            context.BloodLoss += this.BleedingRate.Value;
            this.IsBleeding = !this.BleedingRate.IsAtMinValue();
        }

        public void OnUpdate(InjuryProcessingContext context)
        {
            float healingRate = context.HealingRate.Value * this.HealingRateMultiplier;

            if (healingRate > 0)
            {
                float newHealedAmount = this.HealedAmount + healingRate;
                this.HealedAmount = Math.Min(newHealedAmount, Config.DefaultInjuryHealThreshold);
                float healEffectMultiplier = 1f - (this.HealedAmount / Config.DefaultInjuryHealThreshold);

                float previousPain = this.PainIncrease.Value;
                this.PainIncrease *= healEffectMultiplier;
                float painDiff = previousPain - this.PainIncrease.Value;

                this.BleedingRate *= healEffectMultiplier;

                if (this.BleedingRate.ValueAsPercentage < Config.DefaultInjuryBleedingHealThreshold)
                {
                    this.BleedingRate *= 0f;
                }
                
                context.Pain -= painDiff;
                context.BloodLoss += this.BleedingRate.Value;

                this.IsHealed = this.HealedAmount == Config.DefaultInjuryHealThreshold;
                this.IsBleeding = !this.BleedingRate.IsAtMinValue();

                if (this.IsHealed)
                {
                    this.IsHealedEvent?.Invoke();
                }
            }
        }

        public void OnDestroy(InjuryProcessingContext context)
        {
            context.Pain -= this.PainIncrease.Value;
        }
    }
}