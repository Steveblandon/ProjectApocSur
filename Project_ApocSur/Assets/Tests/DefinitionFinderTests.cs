namespace Projapocsur.Tests
{
    using NUnit.Framework;
    using Projapocsur.Common;
    using Projapocsur;
    using Projapocsur.Things;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class DefinitionFinderTests : TestsBase
    {
        private static readonly string filename = "AllDefs.xml";
        private static readonly string extendedTestDataPath = Path.Combine(TestDataPath, "Definitions");
        private static readonly string defTemplateTestDataPath = Path.Combine(TestDataPath, "DefsTemplate");
        private static readonly string uri = Path.Combine(extendedTestDataPath, filename);

        public override void Setup()
        {
            base.Setup();
            if (!Directory.Exists(extendedTestDataPath))
            {
                Directory.CreateDirectory(extendedTestDataPath);
            }

            if (!Directory.Exists(defTemplateTestDataPath))
            {
                Directory.CreateDirectory(defTemplateTestDataPath);
            }
        }

        [Test]
        public void TestWriteAndRead_EmptyTemplate()
        {
            string emptyUri = Path.Combine(defTemplateTestDataPath, "AllDefs_Empty.xml");
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

            AssertNullExceptionTryCatch<Exception>(() => StorageUtility.SaveData(defs, emptyUri, StorageMode.Absolute));
            AssertNullExceptionTryCatch<Exception>(() => StorageUtility.LoadData(out defs, emptyUri, StorageMode.Absolute));
        }

        [Test]
        public void TestRead_AllDefs()
        {
            var defs = new DefinitionFinder.Defs();

            AssertNullExceptionTryCatch<Exception>(() => StorageUtility.LoadData(out defs, uri, StorageMode.Absolute));
        }
    }
}
