namespace Projapocsur.Tests
{
    public class DefDependantTestsBase : TestsBase
    {
        public override void Setup()
        {
            base.Setup();

            Assert_NoExceptionThrownTryCatch(() => DefinitionFinder.Init(TestDataPath));
        }
    }
}
