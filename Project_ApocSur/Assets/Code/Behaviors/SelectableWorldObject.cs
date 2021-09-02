namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;

    public class SelectableWorldObject : SimpleSelectable
    {
        public static readonly new string CompName = nameof(SelectableWorldObject);

        [Tooltip("Select target outline.")]
        [SerializeField]
        private SpriteRenderer outline;

        [Tooltip("This will be the target outline's color for when this gameobject is selected. It will revert back to the outline's original color when deselected.")]
        [SerializeField]
        private Color onSelectOutlineColor;

        private void Start()
        {
            if (outline != null && onSelectOutlineColor != null)
            {
                this.colorSwitch = new SpriteRendererColorSwitch(outline, onSelectOutlineColor);
            }
        }
    }
}