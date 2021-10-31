namespace Projapocsur.Scripts
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Abstract representation of an object that can be selected. Used to make any gameobject selectable.
    /// </summary>
    public abstract class Selectable : MonoBehaviour, IPointerLeftClickHandler
    {
        public static readonly string CompName = nameof(Selectable);

        public event Action<Selectable> OnSelectStateChangeEvent;

        protected ColorSwitch colorSwitch;

        [Tooltip("This will be the target outline's color for when this gameobject is selected. It will revert back to the outline's original color when deselected.")]
        [SerializeField]
        protected Color onSelectOutlineColor = new Color(1, 1, 1, 1);

        public bool IsSelected { get; protected set; }

        public void OnSelect()
        {
            if (this.IsSelected)
            {
                return;
            }

            this.IsSelected = true;
            this.colorSwitch?.TurnOn();
            this.OnSelectStateChangeEvent?.Invoke(this);

            Debug.Log($"{CompName}: {this.name} selected.");
        }

        public void OnDeselect()
        {
            if (!this.IsSelected)
            {
                return;
            }

            this.IsSelected = false;
            this.colorSwitch?.TurnOff();
            this.OnSelectStateChangeEvent?.Invoke(this);

            Debug.Log($"{CompName}: {this.name} deselected.");
        }

        public virtual void OnPointerLeftClick()
        {
            this.OnSelect();
        }
    }
}