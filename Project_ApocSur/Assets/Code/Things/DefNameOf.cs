namespace Projapocsur.Things
{
    /// <summary>
    /// This serves as a lookup table to help with making sure there is a single place in code to trigger a refactor update 
    /// incase a def name changes. Furthermore, it assures, atleast from code, that the same names aren't being used within 
    /// the same def class.
    /// </summary>
    public static class DefNameOf
    {
        // Note: keep ordered by alphabetical order

        public static class BodyPart
        {
            public static string Human_Arm { get; } = nameof(Human_Arm);

            public static string Human_Head { get; } = nameof(Human_Head);

            public static string Human_Leg { get; } = nameof(Human_Leg);

            public static string Human_Torso { get; } = nameof(Human_Torso);
        }

        public static class Injury
        {
            public static string Bruise { get; } = nameof(Bruise);

            public static string Fracture { get; } = nameof(Fracture);

            public static string Laceration { get; } = nameof(Laceration);
        }

        public static class Stat
        {
            // ===================================================================== // 
            // ============================== MEDICAL ============================== //
            // ===================================================================== // 
            public static string BleedingRate { get; } = nameof(BleedingRate);

            public static string BloodLoss { get; } = nameof(BloodLoss);

            public static string HealingRate { get; } = nameof(HealingRate);

            public static string HitPoints { get; } = nameof(HitPoints);

            public static string Pain { get; } = nameof(Pain);

            public static string PainIncrease { get; } = nameof(PainIncrease);
        }
    }
}
