namespace Projapocsur.Common
{
    using System;

    public static class RandomNumberGenerator
    {
        private static Random random = new Random();

        private static double[] gaussianDistribution = new double[]
        {
            0.0,
            0.1,
            0.2, 0.2,
            0.3, 0.3, 0.3, 0.3,
            0.4, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4,
            0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5,
            0.6, 0.6, 0.6, 0.6, 0.6, 0.6, 0.6, 0.6,
            0.7, 0.7, 0.7, 0.7,
            0.8, 0.8,
            0.9,
            1.0
        };

        /// <summary>
        /// Generates a random value between 0.00 and 1.00.
        /// </summary>
        public static double Roll() => Math.Round(random.NextDouble(), 2, MidpointRounding.AwayFromZero);

        public static double Roll(double min, double max)
        {
            double diff = max - min;
            double rndVal = random.NextDouble();
            double offset = diff * rndVal;

            return Math.Round(min + offset, 2, MidpointRounding.AwayFromZero);
        }

        public static float Roll(float min, float max) => (float) Roll((double)min, (double)max);

        /// <summary>
        /// Generates a standard normally distributed random value between 0.0 and 1.0.
        /// </summary>
        public static double RollGaussian()
        {
            int randValueIndex = random.Next(0, gaussianDistribution.Length);
            double randValue = gaussianDistribution[randValueIndex];
            return randValue;
        }
    }
}