
namespace Projapocsur.Common.Utilities
{
    using UnityEngine;

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class LogUtility
    {
        public static readonly string ClassName = nameof(LogUtility);

        public static void Log(LogLevel logLevel, string message, string source = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                message = $"{source}: message";
            }

            switch(logLevel)
            {
                case LogLevel.Info:
                    Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.LogWarning($"{ClassName}: unknown loglevel {logLevel}. message skipped.");
                    break;
            }
        }
    }
}
