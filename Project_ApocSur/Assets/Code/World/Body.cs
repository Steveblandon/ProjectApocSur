namespace Projapocsur.World
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class Body : Thing<BodyDef>
    {
        private InjuryProcessingContext injuryProcessingContext;

        [XmlMember]
        private BodyHitProcessor hitProcessor;

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
            this.BodyParts = new List<BodyPart>();
            this.injuryProcessingContext = new InjuryProcessingContext(this.Pain, this.BloodLoss, this.HealingRate);
            float heightValue = 0f;
            float heightMultiplier = (float)RandomNumberGenerator.RollGaussian() * this.Def.MaxHeightDeviationFactor * 2 - this.Def.MaxHeightDeviationFactor + 1;
            float bodyPartsAggregateHitpoints = 0f;
            int bodyPartsAggregateSize = 0;

            foreach (var bodyPartDef in this.Def.BodyParts)
            {
                var bodyPart = new BodyPart(bodyPartDef.RefDefName, lengthMultiplier: heightMultiplier);
                bodyPart.OnStart(this.injuryProcessingContext);
                this.BodyParts.Add(bodyPart);
                heightValue = Math.Max(heightValue, bodyPart.FloorHeight);      // note, heightMultiplier will already be factored into the floorHeight, no need to muliply the final heightValue by it
                bodyPartsAggregateHitpoints += bodyPart.HitPoints.Value;
                bodyPartsAggregateSize += bodyPart.Def.Size;
            }

            this.Height = new Stat(DefNameOf.Stat.Height, heightValue, useMinMaxLimiters: false);
            this.HitPoints = new Stat(DefNameOf.Stat.HitPoints, bodyPartsAggregateHitpoints);
            this.BleedingRate = new Stat(DefNameOf.Stat.BleedingRate, 0, maxValue: Config.DefaultBleedingRateOnLimbLoss * bodyPartsAggregateSize);

            // instantiate hit processor given all the established information
            hitProcessor = new BodyHitProcessor(this.Def.StanceCapabilities, this.BodyParts, this.Def.HitBoxCount, this.Height.Value);
        }

        #region stats

        [XmlMember]
        public List<BodyPart> BodyParts { get; protected set; }

        [XmlMember]
        public DefRef<StanceDef> CurrentStance { get; protected set; }

        [XmlMember]
        public Stat BloodLoss { get; protected set; }

        [XmlMember]
        public Stat HealingRate { get; protected set; }

        [XmlMember]
        public Stat Height { get; protected set; }

        [XmlMember]
        public Stat HitPoints { get; protected set; }

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

            if (this.BloodLoss.IsAtMaxValue())
            {
                this.OnDestroy();
                this.IsDestroyed = true;
                return;
            }

            this.BleedingRate -= this.HealingRate.Value;
            this.BloodLoss -= this.HealingRate.Value - this.BleedingRate.Value;

            float maxHitpointsPercentage = 1 - this.BloodLoss.Value / this.BloodLoss.MaxValue;
            float maxHitpointsAllowed = this.HitPoints.MaxValue * maxHitpointsPercentage;
            float bodyPartsAggregateHitPoints = 0f;

            foreach (var bodyPart in this.BodyParts)
            {
                bodyPart.OnUpdate(this.injuryProcessingContext, maxHitpointsPercentage);
                bodyPartsAggregateHitPoints += bodyPart.HitPoints.Value;
            }

            float newHitPoints = Math.Min(bodyPartsAggregateHitPoints, maxHitpointsAllowed);
            this.HitPoints -= this.HitPoints.Value - newHitPoints;
            this.DestroyOnZeroHitPoints();
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

            if (damagedBodyPart == null)
            {
                return;
            }

            if (damagedBodyPart.IsDestroyed)
            {
                damagedBodyPart.OnDestroy(this.injuryProcessingContext);

                // destruction of vital body parts = destroyed body (i.e. death).
                if (damagedBodyPart.Def.IsVital)
                {
                    this.OnDestroy();
                    this.IsDestroyed = true;
                    return;
                }
                else
                {
                    this.BodyParts.RemoveAt(damagedBodyPartIndex);
                    this.hitProcessor.ReCalibrate(this.BodyParts);
                }

                this.BleedingRate += Config.DefaultBleedingRateOnLimbLoss * damagedBodyPart.Def.Size;   // bleeding from limb loss proportional to its size.
            }

            if (!this.IsDestroyed)
            {
                // apply body damage with amplification for vital parts
                float damage = hit.Damage;

                if (damagedBodyPart.Def.IsVital)
                {
                    float remainingBodyPartHitPointsPercentage = damagedBodyPart.HitPoints.Value / damagedBodyPart.HitPoints.MaxValue;
                    int amplification = (int) Math.Max(1, (1 - remainingBodyPartHitPointsPercentage) * 10f);
                    damage *= amplification;
                }

                this.HitPoints -= damage;
                this.DestroyOnZeroHitPoints();
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            this.injuryProcessingContext = new InjuryProcessingContext(this.Pain, this.BloodLoss, this.HealingRate);
        }

        private void DestroyOnZeroHitPoints()
        {
            if (this.HitPoints.IsAtMinValue())
            {
                this.OnDestroy();
                this.IsDestroyed = true;
            }
        }
    }
}