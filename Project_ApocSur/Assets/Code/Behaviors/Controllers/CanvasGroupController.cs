namespace Projapocsur.Behaviors.Controllers
{
    using Projapocsur.Behaviors.UI;
    using Projapocsur.Common;
    using UnityEngine;

    /// <summary>
    /// The component automatically hooks into a <see cref="SelectableUI"/> component to listen for when it's selected/deselected.
    /// It then displays/hides the target view accordingly.
    /// </summary>
    [RequireComponent(typeof(SelectableUI))]
    public class CanvasGroupController : MonoBehaviour
    {
        public static readonly string CompName = nameof(CanvasGroupController);

        [SerializeField]
        private CanvasGroup targetView;

        [Tooltip("Set to true to hide the view by default so that it's only displayed when triggered by the select event.")]
        [SerializeField]
        private bool hideOnStart;

        private SelectableUI triggerButton;

        private void Start()
        {
            if (targetView == null)
            {
                Debug.LogWarning($"{CompName}: missing target view reference for {this.name}");
            }

            if (this.hideOnStart)
            {
                CanvasGroupViewer.Instance.Hide(this.targetView);
            }

            triggerButton = this.GetComponent<SelectableUI>();
            triggerButton.OnSelectStateChangeEvent += this.OnSelectStateChangeEvent;
        }

        private void OnDisable()
        {
            triggerButton.OnSelectStateChangeEvent -= this.OnSelectStateChangeEvent;
        }

        private void OnSelectStateChangeEvent(SimpleSelectable selectable)
        {
            if (triggerButton.IsSelected)
            {
                CanvasGroupViewer.Instance.Show(this.targetView);
            }
            else
            {
                CanvasGroupViewer.Instance.Hide(this.targetView);
            }
        }
    }
}