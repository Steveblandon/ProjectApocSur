namespace Projapocsur
{
    using System;
    using Projapocsur.UI;
    using UnityEngine;

    public class InteractableUIElement : MonoBehaviour, IMouseButtonUpHandler
    {
        public void OnMouseButtonUp(out Action callback)
        {
            callback = OnDeselect;
            Debug.Log($"{this.name} selected");
        }

        public void OnDeselect()
        {
            Debug.Log($"{this.name} deselected");
        }
    }
}
