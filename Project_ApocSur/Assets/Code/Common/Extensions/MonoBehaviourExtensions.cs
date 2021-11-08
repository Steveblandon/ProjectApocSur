namespace Projapocsur
{
    using UnityEngine;

    public static class MonoBehaviourExtensions
    {
        public static bool DisableOnMissingReference<T>(this MonoBehaviour monoBehaviour, T reference, string paramName, string compName, T defaultValue = default(T))
        {
            if (reference == null || reference.Equals(defaultValue))
            {
                Debug.LogError($"Missing reference {paramName}. Disabling {compName} component in {monoBehaviour.name}.");
                monoBehaviour.enabled = false;
                return true;
            }

            return false;
        }
    }
}