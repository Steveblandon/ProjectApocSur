namespace Projapocsur.Scripts
{
    using Projapocsur.Core;
    using UnityEngine;

    /// <summary>
    /// The component automatically hooks into a <see cref="SelectableUI"/> component to listen for when it's selected/deselected.
    /// It then displays/hides the target view accordingly.
    /// </summary>
    [RequireComponent(typeof(SelectableUI))]
    public class CanvasGroupController : MonoBehaviour
    {
        public static readonly string CompName = nameof(CanvasGroupController);

        [Tooltip("The view to display when gameobject is selected and hide when deselected.")]
        [SerializeField]
        private CanvasGroup targetView;

        [Tooltip("Set to true to hide the view by default so that it's only displayed when triggered by the select event.")]
        [SerializeField]
        private bool hideOnStart;

        private SelectableUI triggerButton;

        void Start()
        {
            if (targetView == null)
            {
                Debug.LogWarning($"{CompName}: missing target view reference for {name}");
            }

            if (hideOnStart)
            {
                CanvasGroupViewer.Instance.Hide(targetView);
            }

            triggerButton = GetComponent<SelectableUI>();
            triggerButton.OnSelectStateChangeEvent += OnSelectStateChangeEvent;
        }

        void OnDisable()
        {
            triggerButton.OnSelectStateChangeEvent -= OnSelectStateChangeEvent;
        }

        private void OnSelectStateChangeEvent(Selectable selectable)
        {
            if (triggerButton.IsSelected)
            {
                CanvasGroupViewer.Instance.Show(targetView);
            }
            else
            {
                CanvasGroupViewer.Instance.Hide(targetView);
            }
        }
    }
}