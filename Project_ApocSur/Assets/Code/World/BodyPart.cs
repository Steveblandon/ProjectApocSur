namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;
    using static Projapocsur.World.Config;

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

        public bool IsDestroyed { get; protected set; }

        public IReadOnlyCollection<Injury> Injuries { get => this.injuries; }

        public void OnStart(InjuryProcessingContext context)
        {
            this.Injuries.ForEach(injury => injury.OnStart(context));
            this.latestContext = context;
        }

        public void OnUpdate(InjuryProcessingContext context)
        {
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
                }
            }

            while (unhealedInjuries.Count > 0)
            {
                this.injuries.Enqueue(unhealedInjuries.Dequeue());
            }

            while (newInjuries.Count > 0)
            {
                this.injuries.Enqueue(newInjuries.Dequeue());
            }
        }

        public void OnDestroy(InjuryProcessingContext context)
        {
            this.Injuries.ForEach(injury => injury.OnDestroy(context));
            this.injuries.Clear();
        }

        public void TakeDamage(float damage, IEnumerable<string> injuryDefNames)
        {
            SeverityLevel severity = GetSeverityLevelFromPercentage(damage / this.HitPoints.MaxValue);
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
        }
    }
}