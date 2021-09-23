namespace Projapocsur.Common
{
    using System;

    public static class ExceptionUtility
    {
        public static void TryCatch<T>(Action codeToRun, out T exception) where T : Exception
        {
            exception = null;
            try
            {
                codeToRun();
            }
            catch (T ex)
            {
                exception = ex;
            }
        }
    }
}
