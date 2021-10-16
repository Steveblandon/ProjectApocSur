namespace Projapocsur.Tests
{
    using NUnit.Framework;
    using Projapocsur.World;

    class InjuryProcessingContextTests : DefDependantTestsBase
    {
        [Test]
        public void TestOperators()
        {
            Stat painStat = new Stat(DefNameOf.Stat.Pain, 5, 10);

            InjuryProcessingContext context = new InjuryProcessingContext(painStat, null, null);

            context.Pain += 1;

            Assert.AreEqual(context.Pain.Value, painStat.Value);
        }
    }
}