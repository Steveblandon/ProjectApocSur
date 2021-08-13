namespace Projapocsur.Common
{
    using System;
    using UnityEngine;

    public static class ValidationUtils
    {
        /// <summary>
        /// Validates a string, making sure its not empty, null, or whitespace.
        /// </summary>
        /// <param name="paramName"> The name of the parameter to display in the error log.</param>
        /// <param name="value"> The value to validate.</param>
        public static void ThrowIfStringEmptyNullOrWhitespace(string paramName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(paramName, "can not be null, whitespace, or empty");
            }
        }
    }

}