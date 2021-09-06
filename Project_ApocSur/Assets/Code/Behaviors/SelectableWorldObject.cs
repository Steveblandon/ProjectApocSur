namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine;

    /// <summary>
    /// For use with non-UI game objects. Primarily due to the difference of UIElement's Image component vs. world object's SpriteRenderer.
    /// </summary>
    public class SelectableWorldObject : SimpleSelectable
    {
        public static readonly new string CompName = nameof(SelectableWorldObject);

        [Tooltip("Select target outline.")]
        [SerializeField]
        private SpriteRenderer outline;

        [Tooltip("This will be the target outline's color for when this gameobject is selected. It will revert back to the outline's original color when deselected.")]
        [SerializeField]
        private Color onSelectOutlineColor;

        void Start()
        {
            if (this.outline != null && this.onSelectOutlineColor != null)
            {
                this.colorSwitch = new SpriteRendererColorSwitch(this.outline, this.onSelectOutlineColor);
            }
        }
    }
}