namespace Projapocsur.Behaviors.UI
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Selectable))]
    public class SelectableUI : SimpleSelectable, IPointerClickHandler
    {
        public static readonly new string CompName = nameof(SelectableUI);

        [Tooltip("Select target outline.")]
        [SerializeField]
        private Image outline;

        [Tooltip("This will be the target outline's color for when this gameobject is selected. It will revert back to the outline's original color when deselected.")]
        [SerializeField]
        private Color onSelectOutlineColor;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.OnPointerLeftClick();
            }
        }

        private void Start()
        {
            if (outline != null && onSelectOutlineColor != null)
            {
                this.colorSwitch = new ImageColorSwitch(outline, onSelectOutlineColor);
            }
        }
    }
}
