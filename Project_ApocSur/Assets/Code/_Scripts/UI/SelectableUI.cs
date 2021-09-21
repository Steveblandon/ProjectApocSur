namespace Projapocsur.Scripts
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// For use with UI game objects. Primarily due to the difference of UIElement's Image component vs. world object's SpriteRenderer.
    /// It serves as a simplified version of Unity's <see cref="Selectable"/> type that makes use of <see cref="ColorSwitch"/> to show
    /// an outline and avoids <see cref="Selectable"/>'s overhead.
    /// </summary>
    public class SelectableUI : Selectable, IPointerClickHandler
    {
        public static readonly new string CompName = nameof(SelectableUI);

        [Tooltip("Select target outline.")]
        [SerializeField]
        private Image outline;

        void Start()
        {
            if (this.outline != null && this.onSelectOutlineColor != null)
            {
                this.colorSwitch = new ImageColorSwitch(this.outline, this.onSelectOutlineColor);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.OnPointerLeftClick();
            }
        }
    }
}
