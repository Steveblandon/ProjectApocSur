namespace Projapocsur.Tests
{
    using NUnit.Framework;
    using Projapocsur.Common.Utilities;
    using Projapocsur.Core;
    using Projapocsur.Entities.Definitions;
    using ProjApocSur.Common.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class DefinitionFinderTests : TestsBase
    {
        private static readonly string filename = "AllDefs.xml";
        private static readonly string extendedTestDataPath = Path.Combine(TestDataPath, "Definitions");
        private static readonly string uri = Path.Combine(extendedTestDataPath, filename);

        public override void Setup()
        {
            base.Setup();
            if (!Directory.Exists(extendedTestDataPath))
            {
                Directory.CreateDirectory(extendedTestDataPath);
            }
        }

        [Test]
        public void TestWriteAndRead_EmptyTemplate()
        {
            string emptyUri = Path.Combine(extendedTestDataPath, "AllDefs_Empty.xml");
            var defs = new DefinitionFinder.Defs();

            defs.bodyDefs = new List<BodyDef>();
            defs.bodyPartDefs = new List<BodyPartDef>();
            defs.injuryDefs = new List<InjuryDef>();
            defs.statDefs = new List<StatDef>();

            var bodyDef = new BodyDef();
            bodyDef.BodyParts = new List<DefRef<BodyPartDef>>();
            bodyDef.BodyParts.Add(new DefRef<BodyPartDef>("Human_Leg"));

            defs.bodyDefs.Add(bodyDef);
            defs.bodyPartDefs.Add(new BodyPartDef());
            defs.injuryDefs.Add(new InjuryDef());
            defs.statDefs.Add(new StatDef());
            
            ExceptionUtility.TryCatch(
                () => StorageUtility.SaveData(defs, emptyUri, StorageMode.Absolute),
                out Exception exception);
            Assert.IsNull(exception);

            ExceptionUtility.TryCatch(
                () => StorageUtility.LoadData(out defs, emptyUri, StorageMode.Absolute),
                out exception);
            Assert.IsNull(exception);
        }

        [Test]
        public void TestRead_AllDefs()
        {
            var defs = new DefinitionFinder.Defs();
            ExceptionUtility.TryCatch(
                () => StorageUtility.LoadData(out defs, uri, StorageMode.Absolute),
                out Exception exception);
            Assert.IsNull(exception);
        }
    }
}
