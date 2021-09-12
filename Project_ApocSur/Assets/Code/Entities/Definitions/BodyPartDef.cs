namespace Projapocsur.Entities.Definitions
{
    using System;

    [Serializable]
    public class BodyPartDef : Def
    {
        public int Size;

        public float Height;

        public float HeightOffset;

        public float MaxHitpointsBase;

        public float HealingRateBase;
    }
}
