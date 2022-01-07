namespace Projapocsur.Engine
{
    using System;
    using Projapocsur.Engine.EditorAttributes;
    using UnityEngine;

    /// <summary>
    /// Abstract representation of an object that can be selected. Used to make any gameobject selectable.
    /// </summary>
    public abstract class Selectable : MonoBehaviour, ILeftClickHandler, IRightClickHandler
    {
        public static readonly string CompName = nameof(Selectable);

        public event Action<Selectable> OnSelectStateChangeEvent;

        [SerializeField]
        [ReadOnly]
        private bool isSelected;

        [Tooltip("This will be the target outline's color for when this gameobject is selected. It will revert back to the outline's original color when deselected.")]
        [SerializeField]
        protected Color onSelectOutlineColor = new Color(1, 1, 1, 1);

        protected ColorSwitch colorSwitch;

        public bool IsSelected
        {
            get => this.isSelected;
            protected set => this.isSelected = value;
        }

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

        public virtual void OnLeftClick()
        {
            this.OnSelect();
        }

        public virtual void OnRightClick()
        {
            // TBI
        }
    }
}