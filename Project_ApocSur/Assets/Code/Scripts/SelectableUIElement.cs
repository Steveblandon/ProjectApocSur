namespace Projapocsur
{
    using System;
    using Projapocsur.UI;
    using UnityEngine;

    /// <summary>
    /// Representation of a UI GameObject in the canvas that is selectable. It differentiates itself from
    /// <see cref="EventSystem.Selectable"/> in that these elements can be notified when no UI is selected.
    /// This allows it to disambiguate a deselect as either another element was selected or no UI element was selected.
    /// This disambiguation is carried out via notifications instead of constantly polling the EventSystem.
    /// 
    /// Additionally, this class handles basic toggling of the element's outline so as to indicate visually its been selected.
    /// The toggling is optional, and is handled by adding a <see cref="ImageColorSwitch"/> component to a 
    /// <see cref="Constants.PANEL_OUTLINE_GAMEOBJECT_NAME"/> prefab. So just adding the prefab to an UI element is all that's needed.
    /// 
    /// This class also detaches what is selected from the overseer system (in this case <see cref="UIElementSelector"/>) so that
    /// multiple UI elements can be selected at the same time, but only one (the one referenced by <see cref="UIElementSelector"/>) is
    /// the one in focus.
    /// </summary>
    public class SelectableUIElement : MonoBehaviour, ISelectHandler
    {
        protected ImageColorSwitch PanelOutline { get; private set; }

        /// <summary>
        /// Whether the GameObject representing this UIElement has been selected.
        /// </summary>
        protected bool Selected { get; private set; }

        public static readonly string CompName = nameof(SelectableUIElement);

        public void Start()
        {
            ImageColorSwitch[] toggles = this.GetComponentsInChildren<ImageColorSwitch>();

            foreach(var toggle in toggles)
            {
                if (toggle.name == Constants.PANEL_OUTLINE_GAMEOBJECT_NAME)
                {
                    this.PanelOutline = toggle;
                    break;
                }
            }

            if (PanelOutline == null)
            {
                Debug.LogWarning($"{CompName}: no '{Constants.PANEL_OUTLINE_GAMEOBJECT_NAME}' found on {this.name}");
            }
        }

        public virtual void OnSelect(out Action callback)
        {
            if (!this.Selected)
            {
                callback = OnDeselect;
                this.Selected = true;
                this.PanelOutline.TurnOn();
                Debug.Log($"{CompName}: {this.name} selected");
            }
            else
            {
                callback = null;
            }
        }

        public virtual void OnDeselect()
        {
            if (this.Selected)
            {
                this.Selected = false;
                this.PanelOutline.TurnOff();
                Debug.Log($"{CompName}: {this.name} deselected");
            }
        }
    }
}
