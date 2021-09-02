namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;

    public abstract class SimpleSelectable : MonoBehaviour, IPointerLeftClickHandler
    {
        public static readonly string CompName = nameof(SimpleSelectable);

        public delegate void OnSelectStateChangeEventHandler(SimpleSelectable simpleSelectable);
        public event OnSelectStateChangeEventHandler OnSelectStateChangeEvent;

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