namespace Projapocsur.Behaviors
{
    using System;
    using Projapocsur.Common;
    using UnityEngine;

    /// <summary>
    /// Abstract representation of an object that can be selected. Used to make any gameobject selectable.
    /// </summary>
    public abstract class SimpleSelectable : MonoBehaviour, IPointerLeftClickHandler
    {
        public static readonly string CompName = nameof(SimpleSelectable);

        public event Action<SimpleSelectable> OnSelectStateChangeEvent;

        protected ColorSwitch colorSwitch;

        public bool IsSelected { get; private set; }

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