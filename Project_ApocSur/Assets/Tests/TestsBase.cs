namespace Projapocsur.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using System.IO;

    public class TestsBase : IPrebuildSetup
    {
        public static readonly string TestDataPath = Path.Combine(Application.dataPath, "Tests", "Data");

        public virtual void Setup()
        {
            if (!Directory.Exists(TestDataPath))
            {
                Directory.CreateDirectory(TestDataPath);
            }
        }
    }
}
