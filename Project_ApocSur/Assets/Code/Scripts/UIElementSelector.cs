namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.UI;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIElementSelector : MonoBehaviour
    {
        private GraphicRaycaster rayCaster;
        private PointerEventData pointerEventData;
        private EventSystem eventSystem;
        private StackSet<Action> registeredCallbacks;

        public void Start()
        {
            registeredCallbacks = new StackSet<Action>();
            rayCaster = GetComponent<GraphicRaycaster>();
            eventSystem = (EventSystem)FindObjectOfType(typeof(EventSystem));

            if (rayCaster == null)
            {
                Debug.LogWarning("UIElementSelector: no rayCaster registered");
            }
            if (eventSystem == null)
            {
                Debug.LogWarning("UIElementSelector: no eventSystem registered");
            }
        }

        public void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = Input.mousePosition;
                List<RaycastResult> hitResults = new List<RaycastResult>();
                rayCaster.Raycast(pointerEventData, hitResults);

                if (!hitResults.IsNullOrEmpty())
                {
                    foreach (RaycastResult result in hitResults)
                    {
                        if (result.gameObject.TryGetComponent(out IMouseButtonUpHandler handler))
                        {
                            handler.OnMouseButtonUp(out Action callback);
                            this.registeredCallbacks.Add(callback);
                        }
                    }
                }
                else
                {
                    while (this.registeredCallbacks.Count > 0)
                    {
                        this.registeredCallbacks.Pop()();
                    }
                }
            }
        }
    }
}
