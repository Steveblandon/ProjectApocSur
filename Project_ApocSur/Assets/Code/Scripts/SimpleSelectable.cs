namespace Projapocsur.Scripts
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

        /// <summary>
        /// Called internally when onSelect is called, only executed if instance is not already selected.
        /// </summary>
        /// <remarks>
        /// descendant override expected.
        /// </remarks>
        protected virtual void OnSelectInternal() { }

        /// <summary>
        /// Called internally when onDeselect is called, only executed if instance is not already deselected.
        /// </summary>
        /// <remarks>
        /// descendant override expected.
        /// </remarks>
        protected virtual void OnDeselectInternal() { }
    }
}