namespace Projapocsur.Common.Utility
{
    using System;
    using UnityEngine;

    public static class Utils
    {
        public static void ExecuteWithRetries(Func<bool> executable, int retries = 3)
        {
            for (; !executable() && retries > 0; retries--) ;
        }

        public static T GetSingleComponentInChildrenAndLogOnFailure<T>(Component component, string callerName)
        {
            T[] comps = component.GetComponentsInChildren<T>();
            T comp = comps != null && comps.Length > 0 ? comps[0] : default;

            if (comps != null && comps.Length > 1)
            {
                Debug.LogWarning($"{callerName}: more than one {nameof(T)} comp detected on {component.gameObject.name}'s children. Only one is expected... defaulting to the first one found.");
            }

            if (comp == null)
            {
                Debug.LogWarning($"{callerName}: no {nameof(T)} comp detected on {component.gameObject.name}.");
            }

            return comp;
        }
    }

}