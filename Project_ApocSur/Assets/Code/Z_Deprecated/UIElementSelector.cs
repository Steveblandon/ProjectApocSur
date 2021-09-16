/*namespace Projapocsur.Deprecated
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Registers UI interactions and notifies any UIElements setup to handle these interactions.
    /// </summary>
    public class UIElementSelector : MonoBehaviour
    {
        public static readonly string CompName = nameof(UIElementSelector);

        private GraphicRaycaster rayCaster;
        private EventSystem eventSystem;
        private PointerEventData pointerEventData;
        private StackSet<Action> registeredNoSelectionCallbacks;

        /// <summary>
        /// The currently active UIElementSelector instance. Only one should be active per scene.
        /// </summary>
        public static UIElementSelector Current { get; private set; }

        /// <summary>
        /// References the last UI element hit on pointer click. This means that if multiple selectable UI elements are
        /// stacked at the same location, each gets selected and deselected until the last element is reached. Furthermore, 
        /// if no UI element is clicked, then this value will be null.
        /// </summary>
        public SelectableUI CurrentSelectedUIElement { get; private set; }

        public void OnEnable()
        {
            if (Current == null)
            {
                Current = this;
            }
            else
            {
                Debug.LogError($"{CompName}: attempted to update current, but one already exists. Are there multiple instances?");
            }
        }

        public void Start()
        {
            registeredNoSelectionCallbacks = new StackSet<Action>();
            rayCaster = GetComponent<GraphicRaycaster>();
            eventSystem = (EventSystem)FindObjectOfType(typeof(EventSystem));

            if (rayCaster == null)
            {
                Debug.LogWarning($"{CompName}: no rayCaster registered");
            }
            if (eventSystem == null)
            {
                Debug.LogWarning($"{CompName}: no eventSystem registered");
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

                if (hitResults.IsNullOrEmpty())
                {
                    this.CurrentSelectedUIElement = null;

                    while (this.registeredNoSelectionCallbacks.Count > 0)
                    {
                        this.registeredNoSelectionCallbacks.Pop()();
                    }
                }
                else
                {
                    foreach (RaycastResult result in hitResults)
                    {
                        if (result.gameObject.TryGetComponent(out SelectableUI element))
                        {
                            this.SetSelectedUIElement(element);
                        }
                    }
                }
            }
        }

        public void SetSelectedUIElement(SelectableUI selected)
        {
            this.CurrentSelectedUIElement = selected;
            selected.OnSelect();
            this.registeredNoSelectionCallbacks.Add(selected.OnDeselect);
        }
    }
}
*/