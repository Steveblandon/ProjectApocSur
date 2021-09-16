namespace Projapocsur.Scripts
{
    using Projapocsur.Core;
    using Projapocsur.Entities;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public class DraftButtonController : MonoBehaviour
    {
        public static readonly string CompName = nameof(DraftButtonController);

        private ToggleUI toggleButton;

        void Start()
        {
            toggleButton = GetComponent<ToggleUI>();
            toggleButton.OnSelectStateChangeEvent += OnToggleStateChangeEvent;
            GameManager.Instance.DraftTracker.OnDraftStateChangeEvent += OnDraftStateChangeEvent;
            OnDraftStateChangeEvent(GameManager.Instance.DraftTracker.SelecteesDrafted);
        }

        void OnDisable()
        {
            toggleButton.OnSelectStateChangeEvent -= OnToggleStateChangeEvent;
            GameManager.Instance.DraftTracker.OnDraftStateChangeEvent -= OnDraftStateChangeEvent;
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == false)
            {
                foreach (Character character in GameManager.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = true;
                }
            }
            else if (!toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == true)
            {
                foreach (Character character in GameManager.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = false;
                }
            }
        }

        private void OnDraftStateChangeEvent(bool? state)
        {
            if (toggleButton.IsSelected && (GameManager.Instance.DraftTracker.SelecteesDrafted == false || GameManager.Instance.DraftTracker.SelecteesDrafted == null)
                || !toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == true)
            {
                toggleButton.Toggle();
            }
        }
    }
}