namespace Projapocsur
{
    using UnityEngine;

    public class SimpleSelectable : MonoBehaviour
    {
        public static readonly string CompName = nameof(SimpleSelectable);

        public bool IsSelected { get; private set; }

        public void OnSelect()
        {
            if (!this.IsSelected)
            {
                this.OnSelectInternal();

                this.IsSelected = true;
                Debug.Log($"{CompName}: {this.name} selected.");
            }
        }

        public void OnDeselect()
        {
            if (this.IsSelected)
            {
                this.OnDeselectInternal();

                this.IsSelected = false;
                Debug.Log($"{CompName}: {this.name} deselected.");
            }
        }

        protected virtual void OnSelectInternal() { }

        protected virtual void OnDeselectInternal() { }
    }
}