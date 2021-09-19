namespace Projapocsur.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using System.IO;
    using System;

    public class TestsBase : IPrebuildSetup
    {
        public static readonly string TestDataPath = Path.Combine(Application.dataPath, "Tests", "Data");

        public void Setup()
        {
            if (!Directory.Exists(TestDataPath))
            {
                Directory.CreateDirectory(TestDataPath);
            }
        }
    }
}
