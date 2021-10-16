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
            this.HitPoints = new Stat(DefNameOf.Stat.HitPoints, this.Def.MaxHitPoints);
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

            foreach (var bodyPartDef in this.Def.BodyParts)
            {
                var bodyPart = new BodyPart(bodyPartDef.RefDefName, lengthMultiplier: heightMultiplier);
                bodyPart.OnStart(this.injuryProcessingContext);
                this.BodyParts.Add(bodyPart);
                heightValue = Math.Max(heightValue, bodyPart.FloorHeight);      // note, heightMultiplier will already be factored into the floorHeight, no need to muliply the final heightValue by it
            }

            this.Height = new Stat(DefNameOf.Stat.Height, heightValue, useMinMaxLimiters: false);

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

        #endregion

        public void OnUpdate()
        {
            this.BodyParts.ForEach(bodyPart => bodyPart.OnUpdate(this.injuryProcessingContext));
        }

        public void TakeDamage(BodyHitInfo hit)
        {
            int damagedBodyPartIndex = this.hitProcessor.Process(hit, this.BodyParts, this.CurrentStance);

            if (0 <= damagedBodyPartIndex && damagedBodyPartIndex < this.BodyParts.Count && this.BodyParts[damagedBodyPartIndex].IsDestroyed)
            {
                this.BodyParts[damagedBodyPartIndex].OnDestroy(this.injuryProcessingContext);
                this.BodyParts.RemoveAt(damagedBodyPartIndex);
                this.hitProcessor.ReCalibrate(this.BodyParts);
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            this.injuryProcessingContext = new InjuryProcessingContext(this.Pain, this.BloodLoss, this.HealingRate);
        }
    }
}