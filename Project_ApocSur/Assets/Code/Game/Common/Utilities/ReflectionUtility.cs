namespace Projapocsur
{
    using System;
    using System.Collections.Generic;

    public static class ReflectionUtility
    {
        public static bool IsList(Type genericType, out Type innerType)
        {
            if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(List<>))
            {
                innerType = genericType.GetGenericArguments()[0];
                return true;
            }

            innerType = null;
            return false;
        }
    }
}
