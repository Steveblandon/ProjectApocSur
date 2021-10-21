namespace Projapocsur.World
{
    using System.Collections.Generic;

    public static class Config
    {
        public const float DefaultHealingRateRangeMultiplier = 100;
        public const float DefaultBloodLossThreshold = 100;
        public const float DefaultInjuryHealThreshold = 100;
        public const float DefaultBleedingRateOnLimbLoss = 10;

        public static readonly IReadOnlyDictionary<SeverityLevel, float> SeverityAmpliflier = new Dictionary<SeverityLevel, float>()
        {
            { SeverityLevel.Trivial, 1.0f },
            { SeverityLevel.Minor, 1.25f },
            { SeverityLevel.Major, 1.5f },
            { SeverityLevel.Severe, 2.0f },
        };

        public static readonly IEnumerable<KeyValuePair<float, SeverityLevel>> SeverityThresholds = new List<KeyValuePair<float, SeverityLevel>>()
        {
            new KeyValuePair<float, SeverityLevel>(0.10f, SeverityLevel.Trivial),
            new KeyValuePair<float, SeverityLevel>(0.30f, SeverityLevel.Minor),
            new KeyValuePair<float, SeverityLevel>(0.70f, SeverityLevel.Major),
            new KeyValuePair<float, SeverityLevel>(1.00f, SeverityLevel.Severe)
        };

        public static SeverityLevel GetSeverityLevelFromPercentage(float value)
        {
            foreach (var threshold in SeverityThresholds)
            {
                if (value < threshold.Key)
                {
                    return threshold.Value;
                }
            }

            return SeverityLevel.Severe;
        }
    }
}