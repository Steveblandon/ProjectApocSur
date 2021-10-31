namespace Projapocsur.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    public class RandomNumberGeneratorTest
    {
        [Test]
        public void TestNormality_VisualTest()
        {
            int numbersToGenerate = 20;
            var generatedValues = new List<double>(10);

            while(numbersToGenerate-- > 0)
            {
                double gaussNum = RandomNumberGenerator.RollGaussian();
                Assert.IsTrue(0.0 <= gaussNum && gaussNum <= 1.0);
                generatedValues.Add(gaussNum);
            }

            Assert.AreEqual(generatedValues.Capacity, generatedValues.Count);      // this is meant as the breakpoint line, but also as a pass when running tests in general
        }
    }
}
