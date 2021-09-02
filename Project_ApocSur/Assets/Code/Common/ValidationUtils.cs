namespace Projapocsur.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ValidationUtils
    {
        /// <summary>
        /// Validates a string, making sure its not empty, null, or whitespace.
        /// </summary>
        /// <param name="paramName"> The name of the parameter to display in the error log.</param>
        /// <param name="param"> The value to validate.</param>
        public static void ThrowIfStringEmptyNullOrWhitespace(string paramName, string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException(paramName, "can not be null, whitespace, or empty");
            }
        }

        /// <summary>
        /// Validates an object, making sure its not null.
        /// </summary>
        /// <param name="paramName"> The name of the parameter to display in the error log.</param>
        /// <param name="param"> The object to validate.</param>
        public static void ThrowIfNull(string paramName, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, "can not be NULL.");
            }
        }

        /// <summary>
        /// Validates a container object, making sure its not null or empty.
        /// </summary>
        /// <typeparam name="T"> the data type in the container.</typeparam>
        /// <param name="paramName"> The name of the parameter to display in the error log.</param>
        /// <param name="param"> The parameter to validate.</param>
        public static void ThrowIfNullOrEmpty<T>(string paramName, ICollection<T> param)
        {
            if (param == null || !param.Any())
            {
                throw new ArgumentException(paramName, "can not be null or empty");
            }
        }

        /// <summary>
        /// Validates a container object, making sure its not null or empty.
        /// </summary>
        /// <typeparam name="T"> the data type in the container.</typeparam>
        /// <param name="paramName"> The name of the parameter to display in the error log.</param>
        /// <param name="param"> The parameter to validate.</param>
        public static void ThrowIfNullOrEmpty<T>(string paramName, IEnumerable<T> param)
        {
            if (param == null || !param.Any())
            {
                throw new ArgumentException(paramName, "can not be null or empty");
            }
        }
    }

}