namespace Projapocsur.World
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class Body : Thing<BodyDef>
    {
        private InjuryProcessingContext injuryProcessingContext;

        [XmlMember]
        private BodyHitProcessor hitProcessor;

        [XmlMember]
        private List<BodyPart> bodyParts;

        public Body() { }

        public Body(string defName) : base (defName)
        {
            // instantiate straight-forward stats
            this.BloodLoss = new Stat(DefNameOf.Stat.BloodLoss, 0, Config.DefaultBloodLossThreshold);
            this.Pain = new Stat(DefNameOf.Stat.Pain, 0, this.Def.PainThreshold);
            this.CurrentStance = this.Def.DefaultStance;

            // instantiate healing rate with bounds that give it enough room to be modified by other factors
            float healingRateRange = this.Def.BaseHealingRate * Config.DefaultHealingRateRangeMultiplier;
            this.HealingRate = new Stat(
                DefNameOf.Stat.HealingRate,
                this.Def.BaseHealingRate, 
                maxValue: this.Def.BaseHealingRate + healingRateRange, 
                minValue: this.Def.BaseHealingRate - healingRateRange);

            // instantiate body height using a gaussian based on tallest body part... simultaneously instantiate body parts to determine tallest
            this.bodyParts = new List<BodyPart>();
            this.injuryProcessingContext = new InjuryProcessingContext(this.Pain, this.BloodLoss, this.HealingRate);
            float heightValue = 0f;
            float heightMultiplier = (float)RandomNumberGenerator.RollGaussian() * this.Def.MaxHeightDeviationFactor * 2 - this.Def.MaxHeightDeviationFactor + 1;
            int bodyPartsAggregateSize = 0;

            foreach (var bodyPartDef in this.Def.BodyParts)
            {
                var bodyPart = new BodyPart(bodyPartDef.RefDefName, lengthMultiplier: heightMultiplier);
                bodyPart.OnStart(this.injuryProcessingContext);
                this.bodyParts.Add(bodyPart);
                heightValue = Math.Max(heightValue, bodyPart.FloorHeight);      // note, heightMultiplier will already be factored into the floorHeight, no need to muliply the final heightValue by it
                bodyPartsAggregateSize += bodyPart.Def.Size;
            }

            this.Height = new Stat(DefNameOf.Stat.Height, heightValue, useMinMaxLimiters: false);
            this.HitPointsPercentage = new Stat(DefNameOf.Stat.HitPoints, 1);
            this.BleedingRate = new Stat(DefNameOf.Stat.BleedingRate, 0, maxValue: Config.DefaultBleedingRateOnLimbLoss * bodyPartsAggregateSize);

            // instantiate hit processor given all the established information
            hitProcessor = new BodyHitProcessor(this.Def.StanceCapabilities, this.BodyParts, this.Def.HitBoxCount, this.Height.Value);
        }

        #region stats

        public IReadOnlyList<BodyPart> BodyParts { get => this.bodyParts; }

        [XmlMember]
        public DefRef<StanceDef> CurrentStance { get; protected set; }

        [XmlMember]
        public Stat BloodLoss { get; protected set; }

        [XmlMember]
        public Stat HealingRate { get; protected set; }

        [XmlMember]
        public Stat Height { get; protected set; }

        [XmlMember]
        public Stat HitPointsPercentage { get; protected set; }

        [XmlMember]
        public Stat Pain { get; protected set; }

        [XmlMember]
        public Stat BleedingRate { get; protected set; }    // body can have its own bleeding rate for non-vital limb loss

        #endregion

        public bool IsDestroyed { get; protected set; }     // all body functions should cease if true, that is, no further updates

        public void OnUpdate()
        {
            if (this.IsDestroyed)
            {
                return;
            }

            if (this.BloodLoss.IsAtMaxValue())      // bled to death
            {
                this.DestroySelf();
                return;
            }

            // apply healing rate to blood loss / bleeding rate
            this.BleedingRate -= this.HealingRate.Value;
            this.BloodLoss -= this.HealingRate.Value - this.BleedingRate.Value;

            // calculate max hit points percentage allowed and update body parts
            float maxHitPointsPercentageAllowedByBloodLoss = 1 - this.BloodLoss.ValueAsPercentage;
            float lowestVitalPartHitPointsPercentage = 1f;
            float averagePercentageOfAllBodyPartsHitPoints = 0f;

            foreach (var bodyPart in this.BodyParts)
            {
                bodyPart.OnUpdate(this.injuryProcessingContext, maxHitPointsPercentageAllowedByBloodLoss);
                averagePercentageOfAllBodyPartsHitPoints += bodyPart.HitPoints.ValueAsPercentage;

                if (bodyPart.Def.IsVital)
                {
                    lowestVitalPartHitPointsPercentage = Math.Min(lowestVitalPartHitPointsPercentage, bodyPart.HitPoints.ValueAsPercentage);
                }
            }

            averagePercentageOfAllBodyPartsHitPoints /= this.BodyParts.Count;

            float maxHitpointsAllowedPercentage = Math.Min(averagePercentageOfAllBodyPartsHitPoints, Math.Min(lowestVitalPartHitPointsPercentage, maxHitPointsPercentageAllowedByBloodLoss));
            this.HitPointsPercentage += maxHitpointsAllowedPercentage - this.HitPointsPercentage.Value;   // healing rate does not need to be applied here since it has already been applied to blood loss and body parts

            if (this.HitPointsPercentage.IsAtMinValue())
            {
                this.DestroySelf();
            }
        }

        public void OnDestroy()
        {
            if (this.IsDestroyed)
            {
                return;
            }

            this.BodyParts.ForEach(bodyPart => bodyPart.OnDestroy(this.injuryProcessingContext));
        }

        public void TakeDamage(BodyHitInfo hit)
        {
            if (this.IsDestroyed)
            {
                return;
            }

            var (damagedBodyPart, damagedBodyPartIndex) = this.hitProcessor.ProcessHit(hit, this.BodyParts, this.CurrentStance);

            if (damagedBodyPart != null && damagedBodyPart.IsDestroyed)
            {
                this.RemoveBodyPartAt(damagedBodyPartIndex);
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            this.injuryProcessingContext = new InjuryProcessingContext(this.Pain, this.BloodLoss, this.HealingRate);
        }

        private void RemoveBodyPartAt(int index)
        {
            BodyPart bodyPart = this.bodyParts[index];
            bodyPart.OnDestroy(this.injuryProcessingContext);
            this.bodyParts.RemoveAt(index);

            if (bodyPart.Def.IsVital)    // destruction of vital body parts = destroyed body (i.e. death).
            {
                this.DestroySelf();
            }
            else
            {
                this.BleedingRate += Config.DefaultBleedingRateOnLimbLoss * bodyPart.Def.Size;   // bleeding from limb loss proportional to its size.
                this.hitProcessor.ReCalibrate(this.BodyParts);
            }
        }

        private void DestroySelf()
        {
            this.OnDestroy();
            this.IsDestroyed = true;
        }
    }
}