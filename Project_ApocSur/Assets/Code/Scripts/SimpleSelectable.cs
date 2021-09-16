namespace Projapocsur.Scripts
{
    using Projapocsur.Common;
    using UnityEngine;

    /// <summary>
    /// For use with non-UI game objects. Primarily due to the difference of UIElement's Image component vs. world object's SpriteRenderer.
    /// </summary>
    public class SimpleSelectable : Selectable
    {
        public static readonly new string CompName = nameof(SimpleSelectable);

        [Tooltip("Select target outline.")]
        [SerializeField]
        private SpriteRenderer outline;

        void Start()
        {
            if (this.outline != null && this.onSelectOutlineColor != null)
            {
                this.colorSwitch = new SpriteRendererColorSwitch(this.outline, this.onSelectOutlineColor);
            }
        }
    }
}