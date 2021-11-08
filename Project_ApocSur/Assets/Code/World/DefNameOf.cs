namespace Projapocsur.World
{
    /// <summary>
    /// This serves as a lookup table to help with making sure there is a single place in code to trigger a refactor update 
    /// incase a def name changes. Furthermore, it assures, atleast from code, that the same names aren't being used within 
    /// the same def class.
    /// </summary>
    public static class DefNameOf
    {
        // Note: keep ordered by alphabetical order

        public static class Body
        {
            public const string Human = nameof(Human);
        }

        public static class BodyPart
        {
            public const string Human_Arm = nameof(Human_Arm);

            public const string Human_Head = nameof(Human_Head);

            public const string Human_Leg = nameof(Human_Leg);

            public const string Human_Torso = nameof(Human_Torso);
        }

        public static class Injury
        {
            public const string Bruise = nameof(Bruise);

            public const string Fracture = nameof(Fracture);

            public const string Laceration = nameof(Laceration);
        }

        public static class Stance
        {
            public const string Stand = nameof(Stand);

            public const string Sit = nameof(Sit);

            public const string Crouch = nameof(Crouch);

            public const string SitOnGround = nameof(SitOnGround);

            public const string Prone = nameof(Prone);
        }

        public static class Stat
        {
            // ===================================================================== // 
            // ============================== MEDICAL ============================== //
            // ===================================================================== // 
            public const string BleedingRate = nameof(BleedingRate);

            public const string BloodLoss = nameof(BloodLoss);

            public const string HealingRate = nameof(HealingRate);

            public const string Height = nameof(Height);

            public const string HitPoints = nameof(HitPoints);

            public const string HitPointsPercentage = nameof(HitPointsPercentage);

            public const string Pain = nameof(Pain);

            public const string PainIncrease = nameof(PainIncrease);
        }
    }
}
