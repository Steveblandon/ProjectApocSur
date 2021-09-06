namespace Projapocsur.Common
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Common.Utility;
    using UnityEngine;

    /// <summary>
    /// A messaging system for decoupled communication between different objects.
    /// </summary>
    public class EventManager
    {
        public static readonly string className = nameof(EventManager);

        public static EventManager Instance { get { return _instance; } }
        private static EventManager _instance = new EventManager();
        
        private Dictionary<string, Action> listeners;

        private EventManager()
        {
            this.listeners = new Dictionary<string, Action>();
        }

        /// <summary>
        /// Register a listener to execute a callback when the event is triggered.
        /// </summary>
        /// <param name="eventType">Event Type, see <see cref="EventType"/></param>
        /// <param name="callback"></param>
        /// <remarks> Make sure to unregister listener in Component.OnDisable or Component.OnDestroy when no longer needed.</remarks>
        public void RegisterListener(string eventType, Action callback)
        {
            ValidationUtils.ThrowIfStringEmptyNullOrWhitespace(nameof(eventType), eventType);

            if (callback == null)
            {
                Debug.LogWarning($"{className}: attempted to register listener for eventType:'{eventType}', but no callback was provided");
            }
            else if (listeners.ContainsKey(eventType))
            {
                listeners[eventType] += callback;
            }
            else
            {
                listeners[eventType] = callback;
            }
        }

        public void UnregisterListener(string eventType, Action callback)
        {
            ValidationUtils.ThrowIfStringEmptyNullOrWhitespace(nameof(eventType), eventType);

            if (callback == null)
            {
                Debug.LogWarning($"{className}: attempted to unregister listener for eventType:'{eventType}', but no callback was provided");
            }
            else if (listeners.ContainsKey(eventType))
            {
                if (listeners[eventType] != null)
                {
                    listeners[eventType] -= callback;
                }

                if (listeners[eventType] == null)
                {
                    listeners.Remove(eventType);
                }
            }
        }

        public void TriggerEvent(string eventType)
        {
            ValidationUtils.ThrowIfStringEmptyNullOrWhitespace(nameof(eventType), eventType);

            if (listeners.ContainsKey(eventType))
            {
                listeners[eventType].Invoke();
            }
        }
    }
}
