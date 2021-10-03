namespace Projapocsur.Things
{
    using System.Collections.Generic;
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;
    using static Projapocsur.Things.SeverityConfig;

    [XmlSerializable]
    public class BodyPart : Thing<BodyPartDef>
    {
        private InjuryProcessingContext latestContext;
        private Queue<Injury> newInjuries = new Queue<Injury>();

        [XmlMember]
        private Queue<Injury> injuries = new Queue<Injury>();

        public BodyPart() { }

        public BodyPart(string defName) : base(defName)
        {
            this.HitPoints = new Stat(DefNameOf.Stat.HitPoints, this.Def.MaxHitpoints, true);
        }

        [XmlMember]
        public Stat HitPoints { get; private set; }

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

            foreach (string defName in injuryDefNames)
            {
                var injury = new Injury(defName, severity);
                injury.OnStart(latestContext);
                this.newInjuries.Enqueue(injury);
            }
        }
    }
}