namespace Projapocsur.Common
{
    using System;

    public static class RandomNumberGenerator
    {
        private static Random generator = new Random();

        /// <summary>
        /// Generates a random value between 0.00 and 1.00.
        /// </summary>
        public static double Roll() => Math.Round(generator.NextDouble(), 2, MidpointRounding.AwayFromZero);

        public static double Roll(double min, double max)
        {
            double diff = max - min;
            double rndVal = generator.NextDouble();
            double offset = diff * rndVal;

            return Math.Round(min + offset, 2, MidpointRounding.AwayFromZero);
        }

        public static float Roll(float min, float max) => (float) Roll((double)min, (double)max);

        /// <summary>
        /// Generates a standard normally distributed random value between 0.00 and 1.00.
        /// </summary>
        /// <param name = "mu">Mean of the distribution</param>
        /// <param name = "sigma">Standard deviation</param>
        public static double RollGaussian(double mu = 0, double sigma = 1)
        {
            var u1 = generator.NextDouble();
            var u2 = generator.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            var randNormal = mu + sigma * randStdNormal;

            return Math.Round(randNormal, 2, MidpointRounding.AwayFromZero);
        }
    }
}