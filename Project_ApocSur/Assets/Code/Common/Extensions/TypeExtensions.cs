namespace Projapocsur.Common
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsPrimitive(this Type type) => type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
    }
}