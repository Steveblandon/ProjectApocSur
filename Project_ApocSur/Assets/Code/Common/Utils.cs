namespace Projapocsur
{
    using System;

    public static class Utils
    {
        public static void ExecuteWithRetries(Func<bool> executable, int retries = 3)
        {
            for (; !executable() && retries > 0; retries--) ;
        }
    }

}