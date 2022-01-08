namespace Projapocsur.Engine
{
    using System;
    using UnityEngine;

    public static class KeyCodeExtensions
    {
        public static MouseButton ConvertToMouseButton(this KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                    return MouseButton.Left;
                case KeyCode.Mouse1:
                    return MouseButton.Right;
            }

            throw new NotSupportedException($"no matching conversion rule was found for KeyCode.'{keyCode}'");
        }
    }
}