namespace Projapocsur.Scripts
{
    using Projapocsur;
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public class DraftButtonController : MonoBehaviour
    {
        public static readonly string CompName = nameof(DraftButtonController);

        private ToggleUI toggleButton;

        void Start()
        {
            this.toggleButton = this.GetComponent<ToggleUI>();
            this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;
            GameManager.Instance.DraftTracker.OnDraftStateChangeEvent += this.OnDraftStateChangeEvent;
            this.OnDraftStateChangeEvent(GameManager.Instance.DraftTracker.SelecteesDrafted);
        }

        void OnDisable()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            GameManager.Instance.DraftTracker.OnDraftStateChangeEvent -= this.OnDraftStateChangeEvent;
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == false)
            {
                foreach (Character character in GameManager.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = true;
                }
            }
            else if (!this.toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == true)
            {
                foreach (Character character in GameManager.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = false;
                }
            }
        }

        private void OnDraftStateChangeEvent(bool? state)
        {
            if (this.toggleButton.IsSelected && (GameManager.Instance.DraftTracker.SelecteesDrafted == false || GameManager.Instance.DraftTracker.SelecteesDrafted == null)
                || !this.toggleButton.IsSelected && GameManager.Instance.DraftTracker.SelecteesDrafted == true)
            {
                this.toggleButton.Toggle();
            }
        }
    }
}