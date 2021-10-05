namespace Projapocsur.World
{
    using System.Collections.Generic;

    public class BodyHitInfo
    {
        public BodyHitInfo(float height, IEnumerable<string> injuryDefNames, float damage)
        {
            this.Height = height;
            this.InjuryDefNames = injuryDefNames;
            this.Damage = damage;
        }

        public float Height { get; }

        public IEnumerable<string> InjuryDefNames { get; }

        public float Damage { get; }
    }
}