namespace Projapocsur.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using static Projapocsur.Common.ExceptionUtility;

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

        public static void AssertNullExceptionTryCatch<T>(Action codeToRun) where T : Exception
        {
            TryCatch(codeToRun, out T thrownException);
            
            if (thrownException != null)
            {
                Debug.LogError(thrownException);
                Assert.IsEmpty("exception caught, see console for details");
            }
        }

        public static void AssertNotNullExceptionTryCatch<T>(Action codeToRun, out T thrownException) where T : Exception
        {
            TryCatch(codeToRun, out thrownException);

            Assert.IsNotNull(thrownException);
        }
    }
}
