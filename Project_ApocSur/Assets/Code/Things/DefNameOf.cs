namespace Projapocsur.Things
{
    /// <summary>
    /// This serves as a lookup table to help with making sure there is a single place in code to trigger a refactor update 
    /// incase a def name changes. Furthermore, it assures, atleast from code, that the same names aren't being used within 
    /// the same def class.
    /// </summary>
    public static class DefNameOf
    {
        public static class Stat
        {
            // ===================================================================== // 
            // ============================== MEDICAL ============================== //
            // ===================================================================== // 
            public static string BleedingRate { get; } = nameof(BleedingRate);

            public static string PainIncrease { get; } = nameof(PainIncrease);

            public static string Pain { get; } = nameof(Pain);

            public static string BloodLoss { get; } = nameof(BloodLoss);

            public static string HealingRate { get; } = nameof(HealingRate);
        }

        public static class Injury
        {
            public static string Bruise { get; } = nameof(Bruise);

            public static string Laceration { get; } = nameof(Laceration);

            public static string Fracture { get; } = nameof(Fracture);
        }
    }
}
