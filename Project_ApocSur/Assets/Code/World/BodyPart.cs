namespace Projapocsur.World
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class BodyPart : Thing<BodyPartDef>
    {
        private InjuryProcessingContext latestContext;
        private Queue<Injury> newInjuries = new Queue<Injury>();

        [XmlMember]
        private Queue<Injury> injuries = new Queue<Injury>();

        public BodyPart() { }

        public BodyPart(string defName, float lengthMultiplier = 1f) : base(defName)
        {
            this.HitPoints = new Stat(DefNameOf.Stat.HitPoints, this.Def.MaxHitpoints, true);

            // length multiplier will essentially either shrink or enlarge the body part
            this.Length = this.Def.BaseLength * lengthMultiplier;
            this.FloorOffset = this.Def.BaseFloorOffset * lengthMultiplier;
            this.FloorHeight = this.FloorOffset + this.Length;
        }

        [XmlMember]
        public Stat HitPoints { get; protected set; }

        [XmlMember]
        public float FloorHeight { get; protected set; }

        [XmlMember]
        public float FloorOffset { get; protected set; }

        [XmlMember]
        public float Length { get; protected set; }

        [XmlMember]
        public bool IsDamaged { get; protected set; }

        public bool IsDestroyed { get; protected set; }     // not saving this as it's expected that the body part will be removed once destroyed

        public IReadOnlyCollection<Injury> Injuries { get => this.injuries; }

        public void OnStart(InjuryProcessingContext context)
        {
            this.Injuries.ForEach(injury => injury.OnStart(context));
            this.latestContext = context;
        }

        public void OnUpdate(InjuryProcessingContext context, float maxHitpointsAllowedPercentage = 1f)
        {
            if (this.IsDestroyed)
            {
                return;
            }

            // process injuries
            float injuriesHealedAmount = 0f;
            var unhealedInjuries = new Queue<Injury>();

            while (this.Injuries.Count > 0)
            {
                var injury = this.injuries.Dequeue();

                injury.OnUpdate(context);

                if (injury.IsHealed)
                {
                    injury.OnDestroy(context);
                }
                else
                {
                    unhealedInjuries.Enqueue(injury);
                    injuriesHealedAmount += injury.HealedAmount;
                }
            }

            // requeue unhealed injuries
            while (unhealedInjuries.Count > 0)
            {
                this.injuries.Enqueue(unhealedInjuries.Dequeue());
            }

            // queue new unprocessed injuries, note, this is setup this way so that new injuries from TakeDamage don't interfere with an onUpdate injury processing loop
            while (newInjuries.Count > 0)
            {
                this.injuries.Enqueue(newInjuries.Dequeue());
            }

            // heal up to the allowed max hit points, either by the default passed-in allowed amount or based on how much injuries have healed...
            // ... what we end up with is essentially either body part heals or its healing is stunted (as far as hit points are concerned...
            // ... since injury healing would be independent from this logic). 
            float totalInjuriesHealNeeded = this.injuries.Count * Config.DefaultInjuryHealThreshold;
            
            if (totalInjuriesHealNeeded > 0)
            {
                float injuriesBasedMaxHitPointsAllowedPercentage = injuriesHealedAmount / totalInjuriesHealNeeded;
                maxHitpointsAllowedPercentage = Math.Min(maxHitpointsAllowedPercentage, injuriesBasedMaxHitPointsAllowedPercentage);
            }
            
            float maxHitPointsAllowed = this.HitPoints.MaxValue * maxHitpointsAllowedPercentage;

            this.HitPoints += Math.Min(context.HealingRate.Value, Math.Max(0, maxHitPointsAllowed - this.HitPoints.Value));

            this.IsDamaged = this.HitPoints.Value < this.HitPoints.MaxValue;
        }

        public void OnDestroy(InjuryProcessingContext context)
        {
            this.Injuries.ForEach(injury => injury.OnDestroy(context));
            this.injuries.Clear();
        }

        public void TakeDamage(float damage, IEnumerable<string> injuryDefNames)
        {
            if (this.IsDestroyed)
            {
                return;
            }

            SeverityLevel severity = Config.GetSeverityLevelFromPercentage(damage / this.HitPoints.MaxValue);
            this.HitPoints -= damage;

            if (this.HitPoints.IsAtMinValue())
            {
                this.IsDestroyed = true;
                return;
            }

            foreach (string defName in injuryDefNames)
            {
                var injury = new Injury(defName, severity);
                injury.OnStart(latestContext);
                this.newInjuries.Enqueue(injury);
            }

            this.IsDamaged = true;
        }
    }
}